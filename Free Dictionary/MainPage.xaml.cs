using Microsoft.Maui;
using System.Text.Json;

namespace Free_Dictionary
{
    public partial class MainPage : ContentPage
    {



        private const string DictionaryApiUrl = "https://api.dictionaryapi.dev/api/v2/entries/en/";
       
        public MainPage()
        {
            InitializeComponent();

        }

        private void SubmitButton_Clicked(object sender, EventArgs e)
        {
            string enteredText = textInput.Text;
            if (enteredText != null && enteredText.CompareTo("") != 0)
            {
                bool isInternetAvailable = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (isInternetAvailable)
                {
                    // Internet is available, perform your actions here
                    textInput.IsEnabled = false;
                    massageTxt2.Text = "Meanings";
                    massageTxt.Text = "Loading....";
                    wordText2.Text = "Word";
                    wordText.Text = enteredText.ToUpper();

                    LoadDictionaryData(enteredText);
                    textInput.Text = "";
                }
                else
                {
                    // No internet connection, handle accordingly
                    wordText.Text = "No internet connection";
                }

                
            }
            else { massageTxt.Text = "Its empty"; textInput.IsEnabled = true; }

        }


        private async void LoadDictionaryData(String wordToLookup)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                // string wordToLookup = "hello"; // Replace with the word you want to look up

                HttpResponseMessage response = await httpClient.GetAsync(DictionaryApiUrl + wordToLookup);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Parse the JSON string
                    JsonDocument jsonDocument = JsonDocument.Parse(responseBody);

                    string Meaning = WordMeanings(jsonDocument, 0);
                    JsonElement root = jsonDocument.RootElement[0];
                    JsonElement meaningsElement = root.GetProperty("meanings");
                    // Iterate through each meaning
                    string definitionText = "";
                    int Mcount = 1;
                    foreach (JsonElement meaning in meaningsElement.EnumerateArray())
                    {
                        // Get the "definitions" property
                        JsonElement definitionsElement = meaning.GetProperty("definitions");

                        // Iterate through each definition
                        foreach (JsonElement definition in definitionsElement.EnumerateArray())
                        {
                            // Get the "definition" property value as a string

                            definitionText = definitionText + Mcount++ + ". " + definition.GetProperty("definition").GetString() + "\n\n";

                        }

                    }
                    massageTxt.Text = definitionText;
                    textInput.IsEnabled = true;
                    //massageTxt.Text = Meaning;

                }
                else
                {
                    massageTxt.Text = "Failed to retrieve data from the DictionaryAPI.";
                    textInput.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                massageTxt.Text = "An error occurred: " + ex.Message;
                textInput.IsEnabled = true;
            }
        }


        public String WordMeanings(JsonDocument jsonDocument, int i)
        {

            try
            {
                JsonElement root = jsonDocument.RootElement[i];
                JsonElement meaningsElement = root.GetProperty("meanings");
                JsonElement firstMeaning = meaningsElement.EnumerateArray().First();
                JsonElement definitionsElement = firstMeaning.GetProperty("definitions");
                JsonElement firstDefinition = definitionsElement.EnumerateArray().First();
                string definition = firstDefinition.GetProperty("definition").GetString();

                return definition;
            }
            catch (Exception e)
            {
                return "" + e;
            }
        }

    }
}