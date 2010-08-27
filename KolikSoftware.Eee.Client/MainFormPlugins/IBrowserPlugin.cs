using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KolikSoftware.Eee.Service.Domain;

namespace KolikSoftware.Eee.Client.MainFormPlugins
{
    public interface IBrowserPlugin : IMainFormPlugin
    {
        IList<Post> AllPosts { get; }
        void ScrollDown();
        void AddMessage(Post post, bool initial, bool noResolve);

        void Reload();

        bool CanScroll();

        void SetPostPending(Post post);

        void SetPostSent(Post post);

        void UpdatePost(Post post);
    }
}
