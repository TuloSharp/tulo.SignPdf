#!/usr/bin/env bash
set -e

DESTINATION_PATH=""
NAME=""
PASSWORD=""

while getopts "d:n:p:" opt; do
  case $opt in
    d) DESTINATION_PATH="$OPTARG" ;;
    n) NAME="$OPTARG" ;;
    p) PASSWORD="$OPTARG" ;;
    *) exit 99 ;;
  esac
done

if [ -z "$DESTINATION_PATH" ] || [ ! -d "$DESTINATION_PATH" ]; then
  echo "Invalid destination path"
  exit 1
fi

if [ -z "$NAME" ]; then
  echo "Invalid name"
  exit 2
fi

CONFIG_FILE="$DESTINATION_PATH/$NAME.openssl.conf"
KEY_FILE="$DESTINATION_PATH/$NAME.key"
CRT_FILE="$DESTINATION_PATH/$NAME.crt"
PFX_FILE="$DESTINATION_PATH/$NAME.pfx"

openssl req -x509 -newkey rsa:2048 \
  -keyout "$KEY_FILE" \
  -out "$CRT_FILE" \
  -days 3650 \
  -sha256 \
  -nodes \
  -config "$CONFIG_FILE"

openssl pkcs12 -export \
  -out "$PFX_FILE" \
  -inkey "$KEY_FILE" \
  -in "$CRT_FILE" \
  -password pass:"$PASSWORD"

exit 0