import os
import random
import numpy as np
import pandas as pd
import torch

from sklearn.model_selection import train_test_split
from datasets import Dataset
from transformers import (
    BertTokenizer,
    BertForSequenceClassification,
    Trainer,
    TrainingArguments,
    DataCollatorWithPadding,
)

# ========== SEED FOR REPRODUCIBILITY ==========
seed = 42
random.seed(seed)
np.random.seed(seed)
torch.manual_seed(seed)

# ========== PATH SETUP ==========
base_model_name = "bert-base-uncased"
model_dir = "model/intent"
os.makedirs(model_dir, exist_ok=True)

# ========== LOAD & PREPARE DATA ==========
df = pd.read_csv("data/intent_data.csv", header=0)
df.columns = [col.strip() for col in df.columns]  # Clean column names

if "text" not in df.columns or "label" not in df.columns:
    raise ValueError("CSV must contain 'text' and 'label' columns")

df = df.dropna()
labels = sorted(df["label"].unique())
label2id = {label: i for i, label in enumerate(labels)}
id2label = {i: label for label, i in label2id.items()}
df["label_id"] = df["label"].map(label2id)

dataset = Dataset.from_pandas(df[["text", "label_id"]])
dataset = dataset.rename_columns({"label_id": "labels"})
train_test = dataset.train_test_split(test_size=0.2, seed=seed)

# ========== TOKENIZATION ==========
tokenizer = BertTokenizer.from_pretrained(base_model_name)

def tokenize(example):
    return tokenizer(example["text"], truncation=True)

encoded = train_test.map(tokenize, batched=True)
data_collator = DataCollatorWithPadding(tokenizer=tokenizer)

# ========== MODEL ==========
model = BertForSequenceClassification.from_pretrained(base_model_name, num_labels=len(label2id))
model.config.label2id = label2id
model.config.id2label = id2label

# ========== METRICS ==========
def compute_metrics(eval_pred):
    logits, labels = eval_pred
    preds = np.argmax(logits, axis=-1)
    accuracy = (preds == labels).mean()
    return {"accuracy": accuracy}

# ========== TRAINING SETUP ==========
training_args = TrainingArguments(
    output_dir="training_output/intent",
    evaluation_strategy="epoch",
    per_device_train_batch_size=8,
    per_device_eval_batch_size=8,
    num_train_epochs=3,
    save_total_limit=1,
    save_strategy="epoch",
    logging_dir="training_output/logs",
    logging_steps=20,
    load_best_model_at_end=True,
    metric_for_best_model="accuracy",
    seed=seed,
)

# ========== TRAINER ==========
trainer = Trainer(
    model=model,
    args=training_args,
    train_dataset=encoded["train"],
    eval_dataset=encoded["test"],
    tokenizer=tokenizer,
    data_collator=data_collator,
    compute_metrics=compute_metrics,
)

# ========== TRAIN ==========
trainer.train()

# ========== SAVE ==========
model.save_pretrained(model_dir)
tokenizer.save_pretrained(model_dir)

print(f"✅ Model and tokenizer saved to `{model_dir}`")