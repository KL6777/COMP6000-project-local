#
#
#
from transformers import AutoModelForSequenceClassification, AutoTokenizer
import torch
class InputAnalysis:
    #
    def __init__(self):
        self.model = None
        self.tokenizer = None
    #    
    def load_model(self, model_location):
        self.model = AutoModelForSequenceClassification.from_pretrained(model_location)
        self.tokenizer = AutoTokenizer.from_pretrained(model_location)
        self.model.eval()

    def predict_emotions(self, text):
        # Tokenize the input text
        inputs = self.tokenizer(text, return_tensors="pt", padding="max_length", truncation=True, max_length=128)
        
        # Make predictions
        with torch.no_grad():
            outputs = self.model(**inputs)
        
        # Extract the predicted scores
        scores = outputs.logits.squeeze().tolist()

        # Apply tanh to keep values in the range of -1 to 1
        normalized_scores = torch.tanh(torch.tensor(scores)).tolist()
        print(normalized_scores)
        return normalized_scores
        #return scores
