import logging

import azure.functions as func
import pymongo

from bson.json_util import dumps

def main(req: func.HttpRequest) -> func.HttpResponse:
    uri = "mongodb://hashtagvr:9GD5704BiJfgAhuNrkvvEar4WSGlBGL74tPSAHQEZg3vn6LnPhUN7EBrZAyCvqDT8ygnGLMloo0mMcql3cqh3w==@hashtagvr.documents.azure.com:10255/?ssl=true&replicaSet=globaldb"
    client = pymongo.MongoClient(uri)

    db = client["hashtagvrdb"]
    col = db["objects"]
    
    if req.method == 'GET':
        collectionStr = col.find()
        return func.HttpResponse(f"hello! {dumps(collectionStr)}\n")
    elif req.method == 'POST':
        newDoc = req.get_json()
        col.insert_one(newDoc)
        return func.HttpResponse(f"Request made: {newDoc}\n")
    
    return func.HttpResponse("Bad", 400)