from flask import Flask, request, jsonify
from flask_cors import CORS
from sentence_transformers import SentenceTransformer

app = Flask(__name__)
CORS(app)  # Habilita CORS si accedes desde otras aplicaciones

# 🔹 Carga del modelo de embeddings
model = SentenceTransformer("all-MiniLM-L6-v2")

@app.route("/embedding", methods=["POST"])
def generate_embedding():
    try:
        data = request.get_json()
        text = data.get("text", "").strip()

        if not text:
            return jsonify({"error": "Texto vacío."}), 400

        # 🔹 Generar el embedding
        embedding = model.encode(text).tolist()
        return jsonify({"embedding": embedding})
    except Exception as e:
        return jsonify({"error": str(e)}), 500

if __name__ == "__main__":
    print("Servidor de embeddings iniciado en http://localhost:8001")
    app.run(host="0.0.0.0", port=8001)
