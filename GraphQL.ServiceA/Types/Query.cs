﻿namespace GraphQL.ServiceA.Types
{
    using GraphQL.ServiceA.Models;

    public class Query
    {
        public string Greetings() => "Hello service!";

        public IEnumerable<Comment>? GetComments()
        {
            return new List<Comment>
            {
                new(1, 1, "John", "john@email.com", "message"),
            };
        }
    }
}
