using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace M101DotNet.WebApp.Models
{
    public class Post
    {
        // XXX WORK HERE
        // add in the appropriate properties for a post
        // The homework instructions contain the schema.

        public Post()
        {
            Tags = new List<string>();
            Comments = new List<Comment>();
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Author { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public IList<string> Tags { get; private set; }

        public DateTime CreatedAtUtc { get; set; }

        public IList<Comment> Comments { get; private set; }
    }
}