import hashlib


def handle(req):
    hash_object = hashlib.sha256(str(req).encode('utf-8'))
    hex_dig = hash_object.hexdigest()
    return hex_dig



