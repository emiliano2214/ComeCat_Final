FROM python:3.10-slim

WORKDIR /db

COPY servidorBBDD.py .

RUN pip install flask

EXPOSE 5001

CMD ["python", "servidorBBDD.py"]
