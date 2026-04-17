from fastapi import FastAPI, UploadFile, File, Form
from pydantic import BaseModel
from typing import List
import easyocr
import tempfile
import os

app = FastAPI()

_readers = {}

def get_reader(lang: str):
    lang = (lang or "en").lower()

    if lang not in _readers:
        _readers[lang] = easyocr.Reader([lang], gpu=False)

    return _readers[lang]


class OcrPhraseDto(BaseModel):
    text: str
    x: float
    y: float
    width: float
    height: float
    confidence: float


@app.get("/health")
def health():
    return {"status": "ok"}


@app.post("/ocr", response_model=List[OcrPhraseDto])
async def recognize_text(
    file: UploadFile = File(...),
    lang: str = Form("en")
):
    suffix = os.path.splitext(file.filename or "upload.png")[1] or ".png"

    with tempfile.NamedTemporaryFile(delete=False, suffix=suffix) as tmp:
        content = await file.read()
        tmp.write(content)
        temp_path = tmp.name

    try:
        reader = get_reader(lang)
        results = reader.readtext(temp_path)

        phrases = []
        for item in results:
            bbox, text, confidence = item

            xs = [p[0] for p in bbox]
            ys = [p[1] for p in bbox]

            x = min(xs)
            y = min(ys)
            width = max(xs) - x
            height = max(ys) - y

            phrases.append(
                OcrPhraseDto(
                    text=text,
                    x=float(x),
                    y=float(y),
                    width=float(width),
                    height=float(height),
                    confidence=float(confidence),
                )
            )

        return phrases
    finally:
        if os.path.exists(temp_path):
            os.remove(temp_path)