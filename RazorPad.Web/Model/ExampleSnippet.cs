using System.Collections.Generic;

namespace RazorPad.Web
{
    public class ExampleSnippet : Snippet
    {
        private ExampleSnippet()
        {
        }

        public ExampleSnippet(string key, string title)
        {
            Key = key;
            CreatedBy = "Admin";
            Title = title;
        }
    }

    public class ExampleSnippets : List<Snippet>
    {
        public ExampleSnippets()
        {
            Add(new ExampleSnippet("HelloWorld", "Hello World!") { 
                View = 
@"@{
    var name = ""World"";
}

<h1>Hello, @name!</h1>
"
            });
        }
    }
}