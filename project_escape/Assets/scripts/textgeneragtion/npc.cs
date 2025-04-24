using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Diagnostics;
using System.Transactions;

public class npc : MonoBehaviour
{
    private string _name;
    private string _npcID;
    private Personality _personality;

    private Item _like1;
    private Item _like2;
    private Item _dislike1;
    private Item _dislike2;

    private Dictionary<string, float> _emotions;// = new Dictionary<string, float>();

    private static Dictionary<string, Dictionary<string, float>> _emotionPresets = new Dictionary<string, Dictionary<string, float>>();

    private String _summary;

    public Boolean DynamicEmotion = false;

    private String[][] _emotionalIntensities;

    private string _currentText;

    // being dumb and forgot that unity doesnt use this format when making a new object
    public npc(string name, string npcID , string summary, Personality personality)
    {
        _name = name;
        _npcID = npcID;
        _summary = summary;
        _personality = personality;
        
        /*_emotions.Add("sad_happy", 0);
        _emotions.Add("disgust_trust", 0);
        _emotions.Add("anger_fear", 0);
        _emotions.Add("anticipation_surprise", 0);
        UnityEngine.Debug.Log("emotions:" + _emotions);

        _emotionalIntensities = new String[4][];
        String[] sadHappy = {"grief", "sadness", "pensiveness", "serenity", "joy", "ecstasy"};
        String[] disgustTrust = {"loathing", "disgust", "boredom", "acceptance", "trust", "admiration"};
        String[] angerFear = {"rage", "anger", "annoyance", "apprehension", "fear", "terror"};
        String[] anticipationSurprise = {"vigilance", "anticipation", "interest", "distraction", "surprise", "amazement"};
        _emotionalIntensities[0] = sadHappy;
        _emotionalIntensities[1] = disgustTrust;
        _emotionalIntensities[2] = angerFear;
        _emotionalIntensities[3] = anticipationSurprise;*/
    }

    public void Emotions()
    {
        _emotions = new Dictionary<string, float>();
    }


    //initialize thge npc with the given values 
    public void Initialize(string name, string npcID, string summary, Personality personality)
    {
        _name = name;
        _npcID = npcID;
        _summary = summary;
        _personality = personality;

        _emotions.Add("sad_happy", 0);
        _emotions.Add("disgust_trust", 0);
        _emotions.Add("anger_fear", 0);
        _emotions.Add("anticipation_surprise", 0);
        UnityEngine.Debug.Log("emotions:" + _emotions);

        _emotionalIntensities = new String[4][];
        String[] sadHappy = {"grief", "sadness", "pensiveness", "serenity", "joy", "ecstasy"};
        String[] disgustTrust = {"loathing", "disgust", "boredom", "acceptance", "trust", "admiration"};
        String[] angerFear = {"rage", "anger", "annoyance", "apprehension", "fear", "terror"};
        String[] anticipationSurprise = {"vigilance", "anticipation", "interest", "distraction", "surprise", "amazement"};
        _emotionalIntensities[0] = sadHappy;
        _emotionalIntensities[1] = disgustTrust;
        _emotionalIntensities[2] = angerFear;
        _emotionalIntensities[3] = anticipationSurprise;

        _currentText = "hello i am " + _name;
    }
    
    private void InitializeEmotions()
    {
        _emotions.Add("sad_happy", 0);
        _emotions.Add("disgust_trust", 0);
        _emotions.Add("anger_fear", 0);
        _emotions.Add("anticipation_surprise", 0);
        InitializeEmotionalIntensities();
    }
    /**
        Adds all the emotional intensities from plutchiks wheel to a 2d array which will be used to determine which emotion will be passed to the 
        model for text generation depending on the values of the emotional ranges from the emotions dictionary 
    **/
    private void InitializeEmotionalIntensities(){
        String[] sadHappy = {"grief", "sadness", "pensiveness", "serenity", "joy", "ecstasy"};
        String[] disgustTrust = {"loathing", "disgust", "loathing", "acceptance", "trust", "admiration"};
        String[] angerFear = {"rage", "anger", "annoyance", "apprehension", "fear", "terror"};
        String[] anticipationSurprise = {"vigilance", "anticipation", "interest", "distraction", "surprise", "amazement"};
        _emotionalIntensities[0] = sadHappy;
        _emotionalIntensities[1] = disgustTrust;
        _emotionalIntensities[2] = angerFear;
        _emotionalIntensities[3] = anticipationSurprise;
    }




    public string GetName()
    {
        return _name;
    }

    public string GetID()
    {
        return _npcID;
    }

    public string GetSummary()
    {
        return _summary;
    }

    public string getLike1()
    {
        return _like1.itemName;
    }

    public void SetLike1(Item item)
    {
        _like1 = item;
    }

    public string getLike2()
    {
        if(_like2 != null){
            return _like2.itemName;
        }
        return "";
    }

    public void SetLike2(Item item)
    {
        _like2 = item;
    }

    public string getDislike1()
    {
        return _dislike1.itemName;
    }

    public void SetDislike1(Item item)
    {
        _dislike1 = item;
    }

    public string getDislike2()
    {
        if(_dislike2 != null){
            return _dislike2.itemName;
        }
        return "";
    }

    public void SetDislike2(Item item)
    {
        _dislike2 = item;
    }

    public string GetPersonalityType()
    {
        return _personality.PersonalityType;
    }

    public string GetCurrentText()
    {   
        return _currentText;
    }   

    public void SetCurrentText(string text)
    {
        _currentText = text;
    }

    //converts the values of the emotions dictionary into the emotional intensities from the emotionalIntensities array
    private string[] ConvertEmotionsToString(){
        string[] convertedEmotions = new string[4];
        int counter = 0;
        foreach (KeyValuePair<string, float> kvp in _emotions)
        {
            float value = kvp.Value;
            if (value <= -0.70) convertedEmotions[counter] = _emotionalIntensities[counter][0];
            else if (value <= -0.40) convertedEmotions[counter] = _emotionalIntensities[counter][1];
            else if (value <= -0.10) convertedEmotions[counter] = _emotionalIntensities[counter][2];
            else if (value > -0.10 && value < 0.10) convertedEmotions[counter] = "neutral";
            else if (value < 0.40) convertedEmotions[counter] = _emotionalIntensities[counter][3];
            else if (value < 0.70) convertedEmotions[counter] = _emotionalIntensities[counter][4];
            else convertedEmotions[counter] = _emotionalIntensities[counter][5];
            counter++;
        }
        return convertedEmotions;
    }

    public float GetEmotionValue(string emotion)
    {
        return _emotions[emotion];
    }

    public void SetEmotionValue(string emotion, float value)
    {
        // Creates the Dictionary key if it did not exist
        if (!_emotions.ContainsKey(emotion))
        {
            _emotions.Add(emotion, value);
            UnityEngine.Debug.Log("key not found, making key: " + emotion + " " + _emotions[emotion]);
        }
        // Key is updated if it already exists
        else
        {
            _emotions[emotion] = value;
            UnityEngine.Debug.Log("key found, updating key: " + emotion + " " + _emotions[emotion]);
        }
    }

    //saves the current emotions of an NPC into the emotionPresets dictionary
    //requires a name for the preset
    public void SaveEmotionPreset(string name)
    {
        Dictionary<string, float> emotionsCopy = new Dictionary<string, float>(_emotions);
        _emotionPresets.Add(name, emotionsCopy);
    }
    
    //sets current emotions to specified emotion preset
    public void LoadEmotionPreset(string name)
    {
        _emotions = _emotionPresets[name];
    }

    public Dictionary<string, float> GetEmotionsPreset(string name){
        return _emotionPresets[name];
    }

    //dead real idk what this is i think it was originally used for json serialization but uhh due to inconsistant naming conventions it was never used
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "NPCid", _npcID },
            { "personality", _personality.PersonalityType },
            { "like1", _like1 },
            { "like2", _like2 },
            { "dislike1", _dislike1 },
            { "dislike2", _dislike2 },
            { "Name", _name },
            { "sad_happy", _emotions["sad_happy"] },
            { "disgust_trust", _emotions["disgust_trust"] },
            { "anger_fear", _emotions["anger_fear"] },
            { "anticipation_surprise", _emotions["anticipation_surprise"] }
        };
    }


    //returns the details of the npc in a string format
    public string ReturnDetails()
    {
        var details = new StringBuilder();
    
        details.AppendLine($"Name: {_name}");
        details.AppendLine($"_Summary: {_summary}");
        details.AppendLine("Sad_Happy: " + _emotions["sad_happy"]);
        details.AppendLine("Disgust_Trust: " + _emotions["disgust_trust"]);
        details.AppendLine("Anger_Fear: " + _emotions["anger_fear"]);
        details.AppendLine("Anticipation_Surprise: " + _emotions["anticipation_surprise"]);
        // Return the constructed string
        return details.ToString();
    }

    //updates the emotional parameters of the NPC in the database
    private async Task UpdateEmotions(string analysis)
    {
        String analysisString = analysis.Trim('[', ']');
        String[] analysisStringArray = analysisString.Split(',');
        float[] analysisFloatArray = Array.ConvertAll(analysisStringArray, float.Parse);
        _emotions["sad_happy"] += (float)Math.Round(analysisFloatArray[0] / 10, 2);
        _emotions["disgust_trust"] += (float)Math.Round(analysisFloatArray[1] / 10, 2);
        _emotions["anger_fear"] += (float)Math.Round(analysisFloatArray[2] / 10, 2);
        _emotions["anticipation_surprise"] += (float)Math.Round(analysisFloatArray[3] / 10, 2);

        var requestBody = new {
            NPCid = _npcID,
            sad_happy = _emotions["sad_happy"],
            disgust_trust = _emotions["disgust_trust"],
            anger_fear = _emotions["anger_fear"],
            anticipation_surprise = _emotions["anticipation_surprise"]
        };
        string json = System.Text.Json.JsonSerializer.Serialize(requestBody);
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("http://127.0.0.1:8000");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync("/update_emotional_parameters", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Error: {ex.Message}");
                throw; // Rethrow the exception to allow the calling code to handle it
            }
        }
    }


    //the bread and butter of this project if this doesnt work then the whole pooint of this project is useless 
    //this function sends the player input to the model and recieves a response from the model
    //it also updates the emotional parameters of the NPC based on the player input
    public async Task<string> GenerateReaction(string input) {
        // Use the ConvertEmotionsToString function to convert the values of each emotional range into their respective emotional intensity, returns as an array
        String[] emotes = ConvertEmotionsToString();
        // Using an anonymous type store all of the relevant information in requestBody
        var requestBody = new {
            prompt = input,
            summary = _summary,
            npcid = _npcID,
            emotion1 = emotes[0],
            emotion2 = emotes[1],
            emotion3 = emotes[2],
            emotion4 = emotes[3]
        };  

        var newRequest = new {
            NPCid = _npcID,
            emotion1 = emotes[0],
            emotion2 = emotes[1],
            emotion3 = emotes[2],
            emotion4 = emotes[3],
            message_sent = input,
            return_message = "",
            io_type = "input"
        };

        string jsonrq = System.Text.Json.JsonSerializer.Serialize(newRequest);
        // Convert everything in requestBody to JSON so it can be used in an HTTP request
        string json = System.Text.Json.JsonSerializer.Serialize(requestBody);
        // Create an instance of HttpClient to send and receive HTTP requests
        using (var client = new HttpClient())
        {
            try
            {
                // Set the base address of the client to 127.0.0.1:8000, representing a local server where the FastAPI server will be set up.
                client.BaseAddress = new Uri("http://127.0.0.1:8000");
                // Packages the JSON-serialized data into a format suitable for sending in a POST request to the FastAPI server.
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var contentrq = new StringContent(jsonrq, Encoding.UTF8, "application/json");
                // This is the POST request using /generate which you can see in main.py, it waits for the response before moving on
                HttpResponseMessage response2 = await client.PostAsync("/insert_message_history", contentrq);
                HttpResponseMessage response = await client.PostAsync("/generate", content);
                HttpResponseMessage input_analysis = null;

                if (DynamicEmotion)
                {
                    string jsonInput = System.Text.Json.JsonSerializer.Serialize(new { text = input });
                    var inputForAnalysis = new StringContent(jsonInput, Encoding.UTF8, "application/json");
                    input_analysis = await client.PostAsync("/analyze_player_input", inputForAnalysis);
                }

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    if (DynamicEmotion)
                    {
                        string analysis = await input_analysis.Content.ReadAsStringAsync();
                        await UpdateEmotions(analysis); // Corrected method call
                    }
                    // Convert the result back into a string and return it.
                    string result = await response.Content.ReadAsStringAsync();
                    // Preps another request to insert the response message history into the database
                    var returnmessage = new {
                        NPCid = _npcID,
                        emotion1 = emotes[0],
                        emotion2 = emotes[1],
                        emotion3 = emotes[2],
                        emotion4 = emotes[3],
                        message_sent = "",
                        return_message = result,
                        io_type = "output"
                    }; // Missing semicolon added here
                    string jsonrm = System.Text.Json.JsonSerializer.Serialize(returnmessage);
                    var contentrm = new StringContent(jsonrm, Encoding.UTF8, "application/json");
                    HttpResponseMessage response3 = await client.PostAsync("/insert_message_history", contentrm);
                    UnityEngine.Debug.Log("npc reaction: " + result);
                    return result;
                }
                else
                {
                    // Handle errors
                    return $"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}";
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return $"Error: {ex.Message}";
            }
        }
    }


    //just a test function to see if the npc can recieve an item and change emotions accordingly (can be handled differently by the game sdev so each tem has different values that it will change)
    public async Task<string> ReciveItem(string input , Item item){
        string reaction = "";

        if (item.itemName == _like1.itemName || item.itemName == _like2.itemName)
        {
            reaction = "Thank you for the " + item.itemName + "!";
            _emotions["sad_happy"] += 0.2f;
            _emotions["disgust_trust"] += 0.2f;
            _emotions["anger_fear"] -= 0.2f;
            _emotions["anticipation_surprise"] += 0.2f;
        }
        else if (item.itemName == _dislike1.itemName || item.itemName == _dislike2.itemName)
        {
            reaction = "I don't like " + item.itemName + "!";
            _emotions["sad_happy"] -= 0.2f;
            _emotions["disgust_trust"] -= 0.2f;
            _emotions["anger_fear"] += 0.2f;
            _emotions["anticipation_surprise"] -= 0.2f;
        }
        


        String[] emotes = ConvertEmotionsToString();
        // Using an anonymous type store all of the relevant information in requestBody
        var requestBody = new {
            prompt = input,
            summary = _summary,
            npcid = _npcID,
            emotion1 = emotes[0],
            emotion2 = emotes[1],
            emotion3 = emotes[2],
            emotion4 = emotes[3]
        };  

        var newRequest = new {
            NPCid = _npcID,
            emotion1 = emotes[0],
            emotion2 = emotes[1],
            emotion3 = emotes[2],
            emotion4 = emotes[3],
            message_sent = input,
            return_message = "",
            io_type = "input"
        };

         string jsonrq = System.Text.Json.JsonSerializer.Serialize(newRequest);
        // Convert everything in requestBody to JSON so it can be used in an HTTP request
        string json = System.Text.Json.JsonSerializer.Serialize(requestBody);
        // Create an instance of HttpClient to send and receive HTTP requests
        using (var client = new HttpClient())
        {
            try
            {
                // Set the base address of the client to 127.0.0.1:8000, representing a local server where the FastAPI server will be set up.
                client.BaseAddress = new Uri("http://127.0.0.1:8000");
                // Packages the JSON-serialized data into a format suitable for sending in a POST request to the FastAPI server.
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var contentrq = new StringContent(jsonrq, Encoding.UTF8, "application/json");
                // This is the POST request using /generate which you can see in main.py, it waits for the response before moving on
                HttpResponseMessage response2 = await client.PostAsync("/insert_message_history", contentrq);
                HttpResponseMessage response = await client.PostAsync("/generate", content);
                HttpResponseMessage input_analysis = null;

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    // Preps another request to insert the response message history into the database
                    var returnmessage = new {
                        NPCid = _npcID,
                        emotion1 = emotes[0],
                        emotion2 = emotes[1],
                        emotion3 = emotes[2],
                        emotion4 = emotes[3],
                        message_sent = "",
                        return_message = result,
                        io_type = "output"
                    }; // Missing semicolon added here
                    string jsonrm = System.Text.Json.JsonSerializer.Serialize(returnmessage);
                    var contentrm = new StringContent(jsonrm, Encoding.UTF8, "application/json");
                    HttpResponseMessage response3 = await client.PostAsync("/insert_message_history", contentrm);
                    return result;
                }
                else
                {
                    // Handle errors
                    return $"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}";
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return $"Error: {ex.Message}";
            }
    


        }
    }
}