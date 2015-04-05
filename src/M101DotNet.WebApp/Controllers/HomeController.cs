using M101DotNet.WebApp.Models;
using M101DotNet.WebApp.Models.Home;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace M101DotNet.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var blogContext = new BlogContext();
            // XXX WORK HERE
            // find the most recent 10 posts and order them
            // from newest to oldest

            var sort = Builders<Post>.Sort.Descending(p => p.CreatedAtUtc);
            var recentPosts = await blogContext.Posts.Find(new BsonDocument()).Sort(sort).Limit(10).ToListAsync();

            var model = new IndexModel
            {
                RecentPosts = recentPosts
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult NewPost()
        {
            return View(new NewPostModel());
        }

        [HttpPost]
        public async Task<ActionResult> NewPost(NewPostModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var blogContext = new BlogContext();
            // XXX WORK HERE
            // Insert the post into the posts collection

            var user = Request.GetOwinContext().Authentication.User;
            var post = new Post
            {
                Author = user.Identity.Name,
                CreatedAtUtc = DateTime.Now.ToUniversalTime(),
                Title = model.Title,
                Content = model.Content
            };
            foreach (var tag in model.Tags.Split(','))
            {
                post.Tags.Add(tag);
            }

            await blogContext.Posts.InsertOneAsync(post);

            return RedirectToAction("Post", new { id = post.Id });
        }

        [HttpGet]
        public async Task<ActionResult> Post(string id)
        {
            var blogContext = new BlogContext();

            // XXX WORK HERE
            // Find the post with the given identifier

            var post = await blogContext.Posts.Find(p => p.Id.Equals(id)).SingleOrDefaultAsync();

            if (post == null)
            {
                return RedirectToAction("Index");
            }

            var model = new PostModel
            {
                Post = post
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Posts(string tag = null)
        {
            var blogContext = new BlogContext();

            // XXX WORK HERE
            // Find all the posts with the given tag if it exists.
            // Otherwise, return all the posts.
            // Each of these results should be in descending order.

            var sort = Builders<Post>.Sort.Descending(p => p.CreatedAtUtc);
            var posts = await blogContext.Posts.Find(new BsonDocument()).Sort(sort).ToListAsync();

            return View(posts);
        }

        [HttpPost]
        public async Task<ActionResult> NewComment(NewCommentModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Post", new { id = model.PostId });
            }

            var blogContext = new BlogContext();
            // XXX WORK HERE
            // add a comment to the post identified by model.PostId.
            // you can get the author from "this.User.Identity.Name"

            var user = Request.GetOwinContext().Authentication.User;
            var comment = new Comment
            {
                Author = user.Identity.Name,
                CreatedAtUtc = DateTime.Now.ToUniversalTime(),
                Content = model.Content
            };
            var update = Builders<Post>.Update.Push<Comment>(p => p.Comments, comment);
            await blogContext.Posts.FindOneAndUpdateAsync(p => p.Id.Equals(model.PostId), update);

            return RedirectToAction("Post", new { id = model.PostId });
        }
    }
}