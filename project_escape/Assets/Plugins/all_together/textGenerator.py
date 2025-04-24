#textGenerator class for generating text
#This class will be called from the main.py file which is the fastAPI
#The model will be loaded from the model_location using the load_model function and then text will be generated and returned using the generate_text function
import transformers
from transformers import AutoModelForCausalLM

class TextGenerator:
    #intitialize
    def __init__(self):
        self.model = None
        self.tokenizer = None
        self.pipeline = None
    #load the model tokenizer and pipeline into the corresponding variables
    def load_model(self, model_location):
        self.model = AutoModelForCausalLM.from_pretrained(model_location, device_map="auto")
        self.tokenizer = transformers.AutoTokenizer.from_pretrained(model_location)
        self.pipeline = pipeline = transformers.pipeline(
            'text-generation', 
            model=self.model, 
            tokenizer=self.tokenizer, 
            do_sample=True,
            return_full_text = False
        )
    #generate text
    def generate_text(self, input_text, summary, name, emotion1, emotion2, emotion3, emotion4,history):
        #check to see if the model and tokenizer have been loaded
        if not self.model or not self.tokenizer:
            raise ValueError("Model and tokenizer must be loaded before generating text.")
        #the template is what makes the prompt.
        #this is where we tell the LLM what it is supposed to do, give it a summary, a name and emotions and then the input from the user. (This is also where we will put the conversation history)
        template = (

                "<|begin_of_text|><|start_header_id|>system<|end_header_id|>"
                "You are a character in a story."
                "Use the provided summary, name, and emotions to guide your response."
                "Only provide a diaglogue response."
                "Limit your response to approximately 50 tokens, but ensure it remains coherent."
                f"Your name is {name}."
                f"Summary: {summary}"
                f"You are feeling: {emotion1}, {emotion2}, {emotion3}, {emotion4}"
                f"Conversation history: {history}|eot_id|>"
                "<|start_header_id|>user<|end_header_id|>"
                f"{input_text}<|eot_id|>"
                "<|start_header_id|>assistant<|end_header_id|>"
        )

        sequences = self.pipeline(template, num_return_sequences=1, max_new_tokens=100, return_full_text=False)
        return sequences[0]["generated_text"]
