from fastapi import FastAPI, UploadFile
from sentence_transformers import SentenceTransformer
from PIL import Image
from transformers import BlipProcessor, BlipForConditionalGeneration

app = FastAPI()
st = SentenceTransformer("BAAI/bge-small-en-v1.5", device="cpu")
bp = BlipProcessor.from_pretrained("Salesforce/blip-image-captioning-base")
bm = BlipForConditionalGeneration.from_pretrained("Salesforce/blip-image-captioning-base")

@app.get("/health")
def health_check():
    return "Healthy"

@app.post("/embed-text")
def embed_text(payload: dict):
    v = st.encode(payload["text"], normalize_embeddings=True).tolist()
    return {"vector": v}

@app.post("/embed-image")
async def embed_image(file: UploadFile):
    img = Image.open(file.file).convert("RGB")
    ids = bm.generate(**bp(img, return_tensors="pt"))
    caption = bp.decode(ids[0], skip_special_tokens=True)
    v = st.encode(caption, normalize_embeddings=True).tolist()
    return {"caption": caption, "vector": v}