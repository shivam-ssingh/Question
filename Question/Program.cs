using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Question;
using System.Text;

if (args.Length > 0)
{
    HttpClient client = new HttpClient();

    client.DefaultRequestHeaders.Add("authorization", "Bearer key-value");// enter the key value generated on gpt site
    var isSyntaxQuestion = args[0].ToLower().Contains("syntax");
    if (isSyntaxQuestion)
    {
        args[0] += ".Please only answer with the synatx, no greetings just syntax, don't want any explanation for now! Thanks.";
    }

    var request = new Request();
    request.model = "gpt-3.5-turbo";
    request.messages = new Message[]
    {
        new Message()
        {
            role = "user",
            content = args[0]
        }
    };
    var apiRequest = new StringContent( JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

    var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", apiRequest);
    var responseString = await response.Content.ReadAsStringAsync();

    try
    {
        var gptResponse = JsonConvert.DeserializeObject<Response>(responseString);
        Console.ForegroundColor = ConsoleColor.Green;

        var queryAnswer = gptResponse.choices[0].message.content;
        Console.WriteLine(queryAnswer);
 
        if (isSyntaxQuestion)
        {
            TextCopy.ClipboardService.SetText(queryAnswer);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Looks like you wanted to know a syntax! Its copied to your clipboard. Paste away!");
        }
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Some Issue with gpt! --> {ex}");
    }
}
else
{
    Console.WriteLine("Ask some question");
}
