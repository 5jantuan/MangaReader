from fastapi import FastAPI, UploadFile, File, Form
from pydantic import BaseModel
from typing import List
import easyocr
import tempfile
import os
from PIL import Image

app = FastAPI()

_readers = {}


def get_reader(lang: str):
    lang = "en"

    if lang not in _readers:
        _readers[lang] = easyocr.Reader([lang], gpu=False)

    return _readers[lang]


def resize_image_for_ocr(path: str, max_width: int = 1200):
    img = Image.open(path)

    try:
        original_width = img.width
        original_height = img.height

        if img.width <= max_width:
            return 1.0, 1.0

        ratio = max_width / img.width
        new_width = max_width
        new_height = int(img.height * ratio)

        resized = img.resize((new_width, new_height))
        resized.save(path)

        scale_x_back = original_width / new_width
        scale_y_back = original_height / new_height

        return scale_x_back, scale_y_back

    finally:
        img.close()


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

        scale_x_back, scale_y_back = resize_image_for_ocr(temp_path, max_width=1200)

        results = reader.readtext(
            temp_path,
            canvas_size=1280,
            mag_ratio=1.0
        )

        phrases = []

        for item in results:
            bbox, text, confidence = item

            xs = [p[0] for p in bbox]
            ys = [p[1] for p in bbox]

            x = min(xs) * scale_x_back
            y = min(ys) * scale_y_back
            width = (max(xs) - min(xs)) * scale_x_back
            height = (max(ys) - min(ys)) * scale_y_back

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