# NLP Microservice for ERP Systems

📌 Overview

This microservice is built with FastAPI and integrates HuggingFace Transformers, ONNX Runtime, and PyTorch to power a flexible and intelligent NLP pipeline for enterprise systems like ERP and CRM. It’s designed to interpret user input, classify intents, extract structured data, and process it in real-time — perfect for automating human-like tasks.

⸻

🔧 Features

✅ 1. Named Entity Recognition (NER)
 • Identifies key entities in a sentence.
 • Current support:
 • PER: Person names (e.g., Sarah Johnson)
 • ORG: Organizations (partial support)
 • LOC: Locations (limited)
 • Engine: ONNX-optimized BERT NER model.

✅ 2. Part-of-Speech Tagging (POS)
 • Tags each token in a sentence with its grammatical category (e.g., NOUN, VERB, ADP).
 • Engine: ONNX-optimized POS model.

✅ 3. Sentiment Analysis
 • Classifies messages into:
 • positive
 • neutral
 • negative
 • Useful for prioritizing emotional or urgent content.

✅ 4. Intent Detection
 • Classifies text into predefined actions like:
 • onboard_employee
 • remove_employee
 • generate_report
 • check_payroll
 • create_project
 • Engine: Custom fine-tuned BERT using PyTorch.

✅ 5. Unified Analyze Endpoint
 • /analyze combines NER + POS + Sentiment + Intent into one call
 • Returns fully structured and actionable insights from any sentence.

⸻

📥 API Endpoints

Method Route Description
POST /predict-ner Returns token-wise NER labels
POST /predict-pos Returns POS tags
POST /predict-sentiment Returns sentiment category
POST /predict-intent Returns user intent
POST /analyze Returns structured summary
GET /intents Lists available intents
GET /health Health check for service

⸻

🧠 Training

🔁 Intent Classifier

Trained using a CSV file:

text,label
"Please onboard Sarah Johnson to the HR department",onboard_employee
"Remove John Doe from Finance",remove_employee

To retrain:

python train_intent_model.py

⸻

📁 Directory Structure

FastApi/
├── main.py
├── train_intent_model.py
├── data/
│   └── intent_data.csv
├── model/
│   ├── ner/
│   ├── pos/
│   ├── sentiment/
│   └── intent/
└── ...

⸻

🧪 Sample Response from /analyze

Input:

{ "text": "Please onboard Sarah Johnson to the Marketing department." }

Output:

{
  "entities": {
    "PER": ["Sarah Johnson"]
  },
  "intent": "onboard_employee",
  "sentiment": "neutral",
  "pos_tags": [
    {"token": "please", "label": "INTJ"},
    {"token": "onboard", "label": "VERB"},
    {"token": "sarah", "label": "PROPN"},
    {"token": "johnson", "label": "PROPN"},
    {"token": "marketing", "label": "NOUN"}
  ]
}

⸻

🚧 What’s Next / Future Work

🟡 NER Retraining
 • Current model fails to recognize some entities like “Marketing” as an organization.
 • Plan: build a custom NER dataset for ERP vocabulary and retrain using transformers + datasets.

🟡 📆 Date and Time Extraction
 • Extract dates like “by Monday” or “next week” from user text.
 • Useful for deadline-sensitive commands and scheduling automation.

🟡 🧠 Summarization / Paraphrasing Support
 • Future support to summarize long texts or paraphrase instructions for confirmations.

🟡 🌍 Multi-language Support
 • Expand the pipeline to support French, Spanish, Swahili, etc.
 • Tokenizers and models will adapt based on incoming language.

🟡 🔗 Entity Linking to ERP Modules
 • Automatically link detected entities like Employee, Department, or Date to backend ERP modules.
 • Enables actions like API calls to onboard users, move departments, etc.

🟡 🛠 Modular Pipeline Plug-in
 • Each task (NER, POS, etc.) will become a plug-and-play module.
 • Future users can disable or swap models without touching the core.

⸻

🧪 Tech Stack
 • FastAPI
 • ONNX Runtime
 • HuggingFace Transformers
 • PyTorch
 • NumPy / Pandas
 • Uvicorn (ASGI)

⸻
