using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using System.Diagnostics;

public class OnStart : MonoBehaviour
{
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Rect spawnArea; // Define the spawn area
    [SerializeField] private Sprite[] npcSprites; // List of NPC sprites
    [SerializeField] private Item[] availableItems; // List of available items

    public int npcCount; // Number of NPCs to generate

    public int personalitiesCount; // Number of personalities to generate

    private string[] names = { "John", "Jane", "Alex", "Chris", "Pat" };
    private string[] summaries = { "A friendly NPC", "A curious NPC", "A brave NPC", "A wise NPC", "A mysterious NPC" };

    private Personality[] personalities;

    // Server process for the FastAPI server
    private Process _serverProcess;

    // Counter for generating sequential NPC IDs
    private int npcCounter = 1;


    private void Start()
    {
        ServerSetup(); // Start the FastAPI server
        UnityEngine.Debug.Log("Server started");
        GeneratePersonalities(personalitiesCount); // specified amount of personalities
        GenerateRandomNPCs(npcCount); // Generate spedified amount of nps
    }

    private void Close()
    {
        ServerClose(); // Close the FastAPI server
    }

    public void GeneratePersonalities(int count)
    {
        personalities = new Personality[count];

        string[] personalityTypes = { "Introvert", "Extrovert", "Ambivert" };

        for (int i = 0; i < count; i++)
        {
            string randomPersonalityType = personalityTypes[UnityEngine.Random.Range(0, personalityTypes.Length)];

            float randomSadHappyMultiplier = UnityEngine.Random.Range(-1f, 1f);
            float randomDisgustTrustMultiplier = UnityEngine.Random.Range(-1f, 1f);
            float randomAngerFearMultiplier = UnityEngine.Random.Range(-1f, 1f);
            float randomAnticipationSurpriseMultiplier = UnityEngine.Random.Range(-1f, 1f);

            personalities[i] = new Personality(randomPersonalityType, randomSadHappyMultiplier, randomDisgustTrustMultiplier, randomAngerFearMultiplier, randomAnticipationSurpriseMultiplier);

            UnityEngine.Debug.Log($"Generated Personality: {randomPersonalityType} with Multipliers: SadHappy: {randomSadHappyMultiplier}, DisgustTrust: {randomDisgustTrustMultiplier}, AngerFear: {randomAngerFearMultiplier}, AnticipationSurprise: {randomAnticipationSurpriseMultiplier}");
        }
    }

    public async void GenerateRandomNPCs(int count)
    {
        for (int i = 0; i < count; i++)
        {
            string randomName = names[UnityEngine.Random.Range(0, names.Length)];
            string randomID = Guid.NewGuid().ToString(); // Generate sequential ID
            string randomSummary = summaries[UnityEngine.Random.Range(0, summaries.Length)];

            // Pick a random personality from the array
            Personality randomPersonality = personalities[UnityEngine.Random.Range(0, personalities.Length)];

            // Generate a random position within the spawn area
            Vector2 randomPosition = new Vector2(
                UnityEngine.Random.Range(spawnArea.xMin, spawnArea.xMax),
                UnityEngine.Random.Range(spawnArea.yMin, spawnArea.yMax)
            );

            GameObject npcObject = Instantiate(npcPrefab, randomPosition, Quaternion.identity);
            npc npc = npcObject.GetComponent<npc>();

            npc.Emotions();
            npc.Initialize(randomName, randomID, randomSummary, randomPersonality);

            // Randomize emotions
            npc.SetEmotionValue("sad_happy", UnityEngine.Random.Range(-1f, 1f));
            npc.SetEmotionValue("disgust_trust", UnityEngine.Random.Range(-1f, 1f));
            npc.SetEmotionValue("anger_fear", UnityEngine.Random.Range(-1f, 1f));
            npc.SetEmotionValue("anticipation_surprise", UnityEngine.Random.Range(-1f, 1f));

            // Assign a random sprite to the NPC
            SpriteRenderer spriteRenderer = npcObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && npcSprites.Length > 0)
            {
                spriteRenderer.sprite = npcSprites[UnityEngine.Random.Range(0, npcSprites.Length)];
            }

            // Randomize likes and dislikes
            npc.SetLike1(availableItems[UnityEngine.Random.Range(0, availableItems.Length)]);
            npc.SetLike2(UnityEngine.Random.value > 0.5f ? availableItems[UnityEngine.Random.Range(0, availableItems.Length)] : null);
            npc.SetDislike1(availableItems[UnityEngine.Random.Range(0, availableItems.Length)]);
            npc.SetDislike2(UnityEngine.Random.value > 0.5f ? availableItems[UnityEngine.Random.Range(0, availableItems.Length)] : null);

            // Convert NPC data to dictionary and send to FastAPI server
            Dictionary<string, object> npcData = npc.ToDictionary();
            var npcJson = new
            {
                NPCid = npc.GetID(),
                personality = npc.GetPersonalityType(),
                like1 = npc.getLike1(),
                like2 = npc.getLike2(),
                dislike1 = npc.getDislike1(),
                dislike2 = npc.getDislike2(),
                Name = npc.GetName(),
                sad_happy = npc.GetEmotionValue("sad_happy"),
                disgust_trust = npc.GetEmotionValue("disgust_trust"),
                anger_fear = npc.GetEmotionValue("anger_fear"),
                anticipation_surprise = npc.GetEmotionValue("anticipation_surprise"),
            };
            UnityEngine.Debug.Log("id=" + npcJson.NPCid + " perso= " + npcJson.personality + " like1= " + npcJson.like1 + " like2= " + npcJson.like2 + " dislike1= " + npcJson.dislike1 + " dislike2= " + npcJson.dislike2 + " name= " + npcJson.Name + " sad_happy= " + npcJson.sad_happy + " disgust_trust= " + npcJson.disgust_trust + " anger_fear= " + npcJson.anger_fear + " anticipation_surprise= " + npcJson.anticipation_surprise);
            //UnityEngine.Debug.Log("fails to send data to server");
            string json = System.Text.Json.JsonSerializer.Serialize(npcJson);
            await SendNPCDataToServer(json);
            UnityEngine.Debug.Log($"Generated NPC: {randomName} with ID: {randomID} and Summary: {randomSummary} at Position: {randomPosition}");
        }
    }

    private async Task SendNPCDataToServer(string json)
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("http://127.0.0.1:8000");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync("/add_npc", content);
                response.EnsureSuccessStatusCode();
                UnityEngine.Debug.Log("NPC data sent to server successfully.");
            }
            catch (HttpRequestException httpEx)
            {
                UnityEngine.Debug.LogError($"HTTP Request Error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error sending NPC data to server: {ex.Message}");
            }
        }
    }

    public async void ServerSetup()
    {
        try
        {
            // The cwd should be the root of the unity project and the path to the exe is below
            string cwd = Directory.GetCurrentDirectory();
            string exePath = Path.Combine(cwd, @"Assets\Plugins\all_together\main.exe");

            UnityEngine.Debug.Log($"Current Directory: {cwd}");
            UnityEngine.Debug.Log($"Executable Path: {exePath}");

            // Create a process
            _serverProcess = new Process();
            // Specify the location of the exe
            _serverProcess.StartInfo.FileName = exePath;
            // Starts the exe directly instead of through powershell
            _serverProcess.StartInfo.UseShellExecute = false;
            // Make sure that a window doesn't pop up when the exe starts
            _serverProcess.StartInfo.CreateNoWindow = true;

            _serverProcess.StartInfo.RedirectStandardOutput = true;
            _serverProcess.StartInfo.RedirectStandardError = true;

            // Start the exe
            _serverProcess.Start();

            // wait for the server to be ready
            using (StreamReader reader = _serverProcess.StandardOutput)
            {
                while (!reader.EndOfStream)
                {
                    string output = await reader.ReadLineAsync();
                    UnityEngine.Debug.Log($"Server Output: {output}");
                    if (output.Contains("Server Ready"))  // ✅ Detect the signal
                    {
                        UnityEngine.Debug.Log("Server is ready.");
                        return;
                    }
                }
            }

            // Capture any errors
            using (StreamReader errorReader = _serverProcess.StandardError)
            {
                while (!errorReader.EndOfStream)
                {
                    string errorOutput = await errorReader.ReadLineAsync();
                    UnityEngine.Debug.LogError($"Server Error Output: {errorOutput}");
                }
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error starting server: {ex.Message}");
        }
    }

    // Close the server process
    public void ServerClose()
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("http://127.0.0.1:8000");
            HttpResponseMessage response = client.GetAsync("/shutdown").Result;
        }
    }
}