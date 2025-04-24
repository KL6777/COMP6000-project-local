#This file starts the fastAPI server when you use the command FastApi dev main.py from the command line
from fastapi import FastAPI
from textGenerator import TextGenerator
from input_analysis import InputAnalysis

#mysql.connector is used to connect to the database
import mysql.connector
from mysql.connector import Error

#pydantic is for validating and parsing the data from JSON to a python string
from pydantic import BaseModel

#create the fastAPI thing
app = FastAPI()
#initialize the textGenerator class
text_generator  = TextGenerator()
#
input_analyser = InputAnalysis()

db = None

#connects to the database
try:
    db = mysql.connector.connect(
        host="dragon.kent.ac.uk",
        user="comp6000_14",
        password="za2rypt",
        database="comp6000_14"
    )
    if db.is_connected():
        print("Successfully connected to the database")
except Error as e:
    print(f"Error: {e}")


#This is a pydantic model that can validate incoming requests

class PlayerInput(BaseModel):
    text: str

#When the fastAPI server is started, it will call the load_model function from text_generator.py to load the specified LLM
@app.on_event("startup")
def load_model():
    try:
        text_generator.load_model("Llama-3.2-1B-Instruct")
    except Exception as e:
        raise RuntimeError(f"Failed to load model: {e}")
    try:
        input_analyser.load_model("emotion_model2")
    except Exception as e:
        raise RuntimeError(f"Failed to load model: {e}")
    print("Server Ready")

#This is a pydantic model that can validate incoming requests
class GenerateTextRequest(BaseModel):
    prompt: str
    summary: str
    npcid: str
    emotion1: str
    emotion2: str
    emotion3: str
    emotion4: str 

#When the fastAPI server recieves a /generate post request, we generate a response using the generate_text function from text_generator.py
#fastAPI uses the pydantic model to validate the request and convert it into python strings to be used with text_generator.py
#The response is then automatically converted into JSON and returned
@app.post("/generate")
def generate_text(request: GenerateTextRequest):

    #fetch the NPC's name from the database
    npc_name = fetch_npc_name(request.npcid)

    message_history = fetch_message_history(request.npcid)
    history = ""

    #format history is an understable string
    for message in message_history:
        if message[7] == "input":
            history += "User" + ": " + message[5] + "\n"
        if message[7] == "output":
            history += "system" + ": " + message[6] + "\n"
            

    response = text_generator.generate_text(
        request.prompt,
        request.summary,
        npc_name,
        request.emotion1,
        request.emotion2,
        request.emotion3,
        request.emotion4,
        history
    )
    return response
#this checks the input from the player and returns the emotion scores for the specified pairs
@app.post("/analyze_player_input")
def analyze_player_input(input: PlayerInput):
    """
    Analyze the player's text input and return emotion scores for the specified pairs.
    """
    print(f"Received input: {input.text}")
    inputs = input_analyser.predict_emotions(input.text)
    return inputs

#insert NPC data into the database
class NPCData(BaseModel):
    NPCid: str
    personality: str
    like1: str
    like2: str
    dislike1: str
    dislike2: str
    Name: str
    sad_happy: float
    disgust_trust: float
    anger_fear: float
    anticipation_surprise: float

@app.post("/add_npc")
def insert_npc_data(npc: NPCData):
    print(npc)
    try:
        print(npc)
        cursor = db.cursor()
        insert_query = """
        INSERT INTO npcdata (NPCid, personality, like1, like2, dislike1, dislike2, sad_happy, disgust_trust, anger_fear, anticipation_surprise, Name)
        VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
        """
        data_tuple = (npc.NPCid, npc.personality, npc.like1, npc.like2, npc.dislike1, npc.dislike2, npc.sad_happy, npc.disgust_trust, npc.anger_fear, npc.anticipation_surprise, npc.Name)
        cursor.execute(insert_query, data_tuple)
        db.commit()
        print("Data inserted successfully")
    except Error as e:
        print(f"Error: {e}")
    finally:
        cursor.close()

#when the fastAPI server recieves a /update_emotional_parameters post request, we update the emotional parameters in the database
class EmotionalParameters(BaseModel):
    NPCid: str
    sad_happy: float
    disgust_trust: float
    anger_fear: float
    anticipation_surprise: float

@app.post("/update_emotional_parameters")
def update_emotional_parameters(ep: EmotionalParameters):
    try:
        cursor = db.cursor()
        update_query = """
        UPDATE npcdata
        SET sad_happy = %s, disgust_trust = %s, anger_fear = %s, anticipation_surprise = %s
        WHERE NPCid = %s
        """
        data_tuple = (ep.sad_happy, ep.disgust_trust, ep.anger_fear, ep.anticipation_surprise, ep.NPCid)
        cursor.execute(update_query, data_tuple)
        db.commit()
        print("Emotional parameters updated successfully")
    except Error as e:
        print(f"Error: {e}")
    finally:
        cursor.close()

#clears the NPC data table
@app.post("/clear_table_npcdata")
def clear_table():
    try:
        cursor = db.cursor()
        clear_query = "DELETE FROM npcdata"
        cursor.execute(clear_query)
        db.commit()
        print("Table cleared successfully")
    except Error as e:
        print(f"Error: {e}")
    finally:
        cursor.close()

#clears the message history table
@app.post("/clear_table_message_history")
def clear_table():
    try:
        cursor = db.cursor()
        clear_query = "DELETE FROM `message history`"
        cursor.execute(clear_query)
        db.commit()
        print("Table cleared successfully")
    except Error as e:
        print(f"Error: {e}")
    finally:
        cursor.close()

#When the fastAPI server recieves a /fetch_npc_data post request, we fetch the NPC data from the database
def fetch_npc_data(npc_id):
    try:
        cursor = db.cursor()
        fetch_query = "SELECT * FROM npcdata WHERE NPCid = %s"
        cursor.execute(fetch_query, (npc_id,))
        result = cursor.fetchone()
        result_2d_array = [list(row) for row in result]
        return result_2d_array
    except Error as e:
        print(f"Error: {e}")
    finally:
        cursor.close()

#When the fastAPI server recieves a /fetch_npc_data post request, we fetch the NPC data from the database
def fetch_npc_name(npc_id):
    try:
        cursor = db.cursor()
        fetch_query = "SELECT Name FROM npcdata WHERE NPCid = %s"
        cursor.execute(fetch_query, (npc_id,))
        result = cursor.fetchone()
        return result
    except Error as e:
        print(f"Error: {e}")
    finally:
        cursor.close()


#When the fastAPI server recieves a /fetch_npc_data post request, we fetch the NPC data from the database
def fetch_message_history(npc_id):
    try:
        cursor = db.cursor()
        fetch_query = "SELECT * FROM `message history` WHERE NPCid = %s"
        cursor.execute(fetch_query, (npc_id,))
        result = cursor.fetchall()
        result_2d_array = [list(row) for row in result]
        return result_2d_array
    except Error as e:
        print(f"Error: {e}")
    finally:
        cursor.close()



#When the fastAPI server recieves a /insert_message_history post request, we insert the message history into the database
class NewMessageHistory(BaseModel):
    NPCid: str
    emotion1: str
    emotion2: str
    emotion3: str
    emotion4: str
    message_sent: str
    return_message: str
    io_type: str

@app.post("/insert_message_history")
def insert_message_history(newMH: NewMessageHistory):
    try:
        cursor = db.cursor()
        insert_query = """
        INSERT INTO `message history` (NPCid, emotion1, emotion2, emotion3, emotion4, `message sent`, `return message`, `input/output`)
        VALUES (%s, %s, %s, %s, %s, %s, %s, %s)
        """
        data_tuple = (newMH.NPCid, newMH.emotion1, newMH.emotion2, newMH.emotion3, newMH.emotion4, newMH.message_sent, newMH.return_message, newMH.io_type)
        cursor.execute(insert_query, data_tuple)
        db.commit()
        print("Message history inserted successfully")
    except Error as e:
        print(f"Error: {e}")
    finally:
        cursor.close()

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="127.0.0.1", port=8000, log_level="info")