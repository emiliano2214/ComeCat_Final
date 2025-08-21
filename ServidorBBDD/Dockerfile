FROM python:3.10-slim

WORKDIR /db

COPY servidorBBDD.py .

# Actualizar repos y agregar curl
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Instalar Flask
RUN pip install flask

EXPOSE 5002

CMD ["python", "servidorBBDD.py"]
