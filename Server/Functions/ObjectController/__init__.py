import azure.functions as func
import pymongo
from bson.json_util import dumps

def main(req: func.HttpRequest) -> func.HttpResponse:
    uri = "mongodb://hashtagvr:9GD5704BiJfgAhuNrkvvEar4WSGlBGL74tPSAHQEZg3vn6LnPhUN7EBrZAyCvqDT8ygnGLMloo0mMcql3cqh3w==@hashtagvr.documents.azure.com:10255/?ssl=true&replicaSet=globaldb"
    client = pymongo.MongoClient(uri)

    reqBody = req.get_json()
    location = reqBody["location"]
    hashtag = reqBody["hashtag"]

    db = client["hashtagvrdb"]
    col = db["objects"]

    if req.method == 'GET':
        collectionStr = col.find(f'{{"hashtag":{hashtag}}}')
        return func.HttpResponse(f"{dumps(collectionStr)}\n")
    elif req.method == 'POST':
        objType = reqBody["objType"]
        source = reqBody["source"]
        col.insert_one(f'{{"hashtag":"{hashtag}", "location":"{location}", "objType":"{objType}", "source":"{source}"}}')
        return func.HttpResponse(f"Request made: {newDoc}\n")
    
    return func.HttpResponse("Bad request!", 400)