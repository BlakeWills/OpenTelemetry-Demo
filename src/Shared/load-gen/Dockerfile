FROM python:3

WORKDIR /usr/src/app

COPY load-generator.py ./
RUN pip install --no-cache-dir requests

CMD [ "python", "-W", "ignore", "./load-generator.py" ]