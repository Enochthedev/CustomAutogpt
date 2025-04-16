from fastapi import FastAPI
from pydantic import BaseModel
import onnxruntime as ort
import numpy as np
import json
from pathlib import Path
from transformers import AutoTokenizer, AutoModelForSequenceClassification
import torch

app = FastAPI()
BASE_DIR = Path(__file__).parent

class TextRequest(BaseModel):
    text: str

# === NER ===
ner_tokenizer = AutoTokenizer.from_pretrained(BASE_DIR / "model/ner")
ner_session = ort.InferenceSession(str(BASE_DIR / "model/ner/model.onnx"))
with open(BASE_DIR / "model/ner/config.json") as f:
    ner_labels = json.load(f)["id2label"]

# === POS ===
pos_tokenizer = AutoTokenizer.from_pretrained(BASE_DIR / "model/pos")
pos_session = ort.InferenceSession(str(BASE_DIR / "model/pos/model.onnx"))
with open(BASE_DIR / "model/pos/config.json") as f:
    pos_labels = json.load(f)["id2label"]

# === Sentiment ===
sentiment_tokenizer = AutoTokenizer.from_pretrained(BASE_DIR / "model/sentiment")
sentiment_session = ort.InferenceSession(str(BASE_DIR / "model/sentiment/model.onnx"))
with open(BASE_DIR / "model/sentiment/config.json") as f:
    sentiment_labels = json.load(f)["id2label"]

# === Intent ===
intent_tokenizer = AutoTokenizer.from_pretrained(BASE_DIR / "model/intent")
intent_model = AutoModelForSequenceClassification.from_pretrained(BASE_DIR / "model/intent")
intent_model.eval()
intent_labels = {str(k): v for k, v in intent_model.config.id2label.items()}

# === Helper: ONNX classification ===
def classify_onnx(text, tokenizer, session, labels):
    tokens = tokenizer(text, return_tensors="np", truncation=True, padding="max_length", max_length=128)
    ort_inputs = {k: v for k, v in tokens.items()}
    if "token_type_ids" not in ort_inputs:
        ort_inputs["token_type_ids"] = np.zeros_like(ort_inputs["input_ids"])
    output = session.run(None, ort_inputs)[0]
    preds = np.argmax(output, axis=-1)[0]
    tokens = tokenizer.convert_ids_to_tokens(ort_inputs["input_ids"][0])
    return [{"token": tok, "label": labels[str(preds[i])]} for i, tok in enumerate(tokens) if tok not in tokenizer.all_special_tokens]

def extract_entities_from_ner(tokens_and_labels):
    entities = {}
    current_entity = ""
    current_label = None

    for item in tokens_and_labels:
        token = item["token"]
        label = item["label"]

        if label.startswith("B-"):
            if current_entity:
                entities.setdefault(current_label, []).append(current_entity.strip())
            current_label = label[2:]
            current_entity = token if not token.startswith("##") else token[2:]

        elif label.startswith("I-") and current_label == label[2:]:
            if token.startswith("##"):
                current_entity += token[2:]
            else:
                current_entity += " " + token

        else:
            if current_entity:
                entities.setdefault(current_label, []).append(current_entity.strip())
                current_entity = ""
            current_label = None

    if current_entity:
        entities.setdefault(current_label, []).append(current_entity.strip())

    return entities

def _join_tokens(tokens):
    # Fix WordPiece joining (e.g., ["H", "##R"] -> "HR")
    joined = ""
    for tok in tokens:
        if tok.startswith("##"):
            joined += tok[2:]
        elif len(joined) > 0:
            joined += " " + tok
        else:
            joined = tok
    return joined


@app.post("/predict-ner")
def predict_ner(req: TextRequest):
    return {"entities": classify_onnx(req.text, ner_tokenizer, ner_session, ner_labels)}

@app.post("/predict-pos")
def predict_pos(req: TextRequest):
    return {"tags": classify_onnx(req.text, pos_tokenizer, pos_session, pos_labels)}

@app.post("/predict-sentiment")
def predict_sentiment(req: TextRequest):
    inputs = sentiment_tokenizer(
        req.text,
        return_tensors="np",
        truncation=True,
        padding="max_length",
        max_length=128,
    )

    if "token_type_ids" not in inputs:
        inputs["token_type_ids"] = np.zeros_like(inputs["input_ids"])

    outputs = sentiment_session.run(None, inputs)[0]  # ✅ no third argument
    pred = np.argmax(outputs, axis=-1)[0]
    return {"sentiment": sentiment_labels[str(pred)]}

@app.post("/predict-intent")
def predict_intent(req: TextRequest):
    inputs = intent_tokenizer(req.text, return_tensors="pt", truncation=True, padding=True)
    with torch.no_grad():
        logits = intent_model(**inputs).logits
    pred = torch.argmax(logits, dim=-1).item()
    return {"intent": intent_labels[str(pred)].strip()}


@app.post("/analyze")
def analyze_text(req: TextRequest):
    ner_tokens = classify_onnx(req.text, ner_tokenizer, ner_session, ner_labels)
    pos_tags = classify_onnx(req.text, pos_tokenizer, pos_session, pos_labels)
    intent_result = predict_intent(req)

    structured_entities = extract_entities_from_ner(ner_tokens)

    return {
        "entities": structured_entities,
        "intent": intent_result["intent"],
        "pos_tags": pos_tags
    }

@app.get("/health")
def health():
    return {"status": "ok"}

@app.get("/intents")
def get_intents():
    return {"intents": list(intent_labels.values())}