import azure.functions as func
import pymongo
from bson.json_util import dumps

def main(req: func.HttpRequest) -> func.HttpResponse:
    uri = "mongodb://hashtagvr:9GD5704BiJfgAhuNrkvvEar4WSGlBGL74tPSAHQEZg3vn6LnPhUN7EBrZAyCvqDT8ygnGLMloo0mMcql3cqh3w==@hashtagvr.documents.azure.com:10255/?ssl=true&replicaSet=globaldb"
    client = pymongo.MongoClient(uri)

    headers = req.headers
    
    location = headers["location"]
    hashtag = headers["hashtag"]

    db = client["hashtagvrdb"]
    col = db["objects"]

    if req.method == 'GET':
        collectionStr = col.find({"hashtag":hashtag})
        return func.HttpResponse(f"{dumps(collectionStr)}\n")
    elif req.method == 'POST':
        objType = headers["objType"]
        source = headers["source"]
        insertionDict = {"hashtag":hashtag, "location":location, "objType":objType, "source":source}
        col.insert_one(insertionDict)
        return func.HttpResponse(f"Request made: {str(insertionDict)}\n")
    
    return func.HttpResponse("Bad request!", 400)