using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using MonoTouch.CoreGraphics;
using System.Collections.Generic;
using MonoTouch.UIKit;

namespace Spearware.iOS
{
    //  public interface
    public sealed class ScrollPage : IDisposable
    {
        public UIViewController ViewController { get; set; }

        public int Index { get; set; }

        public void Dispose()
        {
            if (ViewController != null)
            {
                ViewController.View.RemoveFromSuperview();
                ViewController.View.Dispose();
                ViewController.RemoveFromParentViewController();
                ViewController.Dispose();
            }
        }
    }
    /// <summary>
    /// Allows client class to configure a scrollpage. 
    /// </summary>
    public delegate void ConfigurePageDelegate(ScrollPage scrollPage);
    /// <summary>
    /// User interface pageing scroll view.
    /// </summary>
    public sealed class UIPageingScrollView : IDisposable
    {
        /// <summary>
        /// ScorllView that host the paged items.
        /// </summary>
        /// <value>The scroll view.</value>
        UIScrollView ScrollView { get; set; }

        /// <summary>
        /// The parent view controller. ScrollView will be placed on this ViewController's view
        /// </summary>
        readonly UIViewController ParentViewController;
        /// <summary>
        /// The size of the parent.
        /// </summary>
        readonly SizeF ParentSize;

        /// <summary>
        /// To configure a page
        /// </summary>
        readonly ConfigurePageDelegate ConfigurePage;

        /// <summary>
        /// A stack of pages that can be recycled
        /// </summary>
        readonly Stack<ScrollPage> RecyclePages = new Stack<ScrollPage>();

        /// <summary>
        /// What pages that are currently visible
        /// </summary>
        readonly IList<ScrollPage> VisiblePages = new List<ScrollPage>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Spearware.iOS.UIPageingScrollView"/> class.
        /// </summary>
        /// <param name="parentViewController">ParentViewController that is hosting this Paging ScrollView</param>
        /// <param name="configurePage">Configures/Creates a new ScollPage. Creating the ViewController and view that is hosted in the ScrollView</param>
        public UIPageingScrollView(UIViewController parentViewController, ConfigurePageDelegate configurePage)
        {
            if (parentViewController == null)
                throw new ArgumentNullException("parentViewController");
            if (configurePage == null)
                throw new ArgumentNullException("configurePage");
            ParentViewController = parentViewController;
            ConfigurePage = configurePage;
            ParentSize = parentViewController.View.Frame.Size;
            Load();
        }

        /// <summary>
        /// The content size needed by the UIScrollView.
        /// </summary>
        /// <value>The size of the content.</value>
        SizeF ContentSize
        {
            get
            {
                return new SizeF(ParentSize.Width * NumberOfPages, ParentSize.Height);
            }
        }

        /// <summary>
        /// Sets the content size on the hosted UIScrollView
        /// </summary>
        void UpdateContentSize()
        {
            if (ScrollView != null)
            {
                ScrollView.ContentSize = ContentSize;
            }
        }

        /// <summary>
        /// Setup our internal UIScrollView
        /// </summary>
        void Load()
        {
            Debug.Assert(ScrollView == null, "ScrollView should only be set once.");

            ScrollView = new UIScrollView
            {
                PagingEnabled = true,
                ContentSize = ContentSize,
            };

            ScrollView.Scrolled += HandleScrolled;

            ParentViewController.View.AddSubview(ScrollView);

            new ConstraintBuilder(ParentViewController.View, ScrollView)
                .WithParentTop()
                .WithParentBottom()
                .WithRight()
                .WithLeft()
                .Apply();

            ParentViewController.View.LayoutIfNeeded();

            TilePages();
        }

        void HandleScrolled(object sender, EventArgs e)
        {
            TilePages();
        }

        #region Paging

        int _numberOfPages;

        public int NumberOfPages
        {
            get { return _numberOfPages; }
            set
            {
                _numberOfPages = value;
                UpdateContentSize();
                TilePages();
            }
        }

        ScrollPage DequePage()
        {
            return RecyclePages.Count > 0 ? RecyclePages.Pop() : new ScrollPage();
        }

        public void TilePages()
        {
            int first, last;
            CalcPageIndex(out first, out last);
            Recyle(first, last);
        }

        /// <summary>
        /// Calculates the first and last page index for the scrollview state
        /// </summary>
        /// <param name="first">First.</param>
        /// <param name="last">Last.</param>
        void CalcPageIndex(out int  first, out int last)
        {
            var visibleBounds = ScrollView.Bounds;
            first = (int)Math.Floor(visibleBounds.GetMinX() / visibleBounds.Width);
            last = (int)Math.Floor((visibleBounds.GetMaxX() - 1) / visibleBounds.Width);
            first = Math.Max(first, 0);
            last = Math.Min(last, NumberOfPages - 1);
        }

        /// <summary>
        /// Recyle the pages between fromIndex and toIndex for future use.
        /// </summary>
        /// <param name="fromIndex">From index.</param>
        /// <param name="toIndex">To index.</param>
        void RecylePages(int fromIndex, int toIndex)
        {
            //Remove pages from the ViewController, and Views placing in the recycle set for future use.

            var pagesToRecyle = (from v in VisiblePages
                                          where v.Index < fromIndex || v.Index > toIndex
                                          select v).ToList();

            foreach (var page in pagesToRecyle)
            {
                Console.WriteLine("Recycle page");
                RecyclePages.Push(page);
                VisiblePages.Remove(page);
            }
        }

        /// <summary>
        /// Recycle views that are not needed for the currnet visible page indexes
        /// </summary>
        /// <param name="firstIndex">First index.</param>
        /// <param name="lastIndex">Last index.</param>
        void Recyle(int firstIndex, int lastIndex)
        {
            RecylePages(firstIndex, lastIndex);

            for (int i = firstIndex; i <= lastIndex; i++)
            {
                if (IsDisplayingPageForIndex(i))
                    continue;

                var scrollPage = DequePage();

                var needToAddToView = (scrollPage.ViewController == null);

                scrollPage.Index = i;
                ConfigurePage(scrollPage);

                var view = scrollPage.ViewController.View;

                if (needToAddToView)
                {
                    ScrollView.AddSubview(scrollPage.ViewController.View);
                    ParentViewController.AddChildViewController(scrollPage.ViewController);
                }

                view.Frame = new RectangleF(ParentSize.Width * i, 0, ScrollView.Frame.Size.Width, ScrollView.Frame.Size.Height);

                VisiblePages.Add(scrollPage);
            }
        }

        bool IsDisplayingPageForIndex(int index)
        {
            return (VisiblePages.Any(p => p.Index == index));
        }

        #endregion

        #region Clean Up

        public void Dispose()
        {

            foreach (var scrollView in RecyclePages)
            {
                scrollView.Dispose();
            }

            foreach (var scrollView in VisiblePages)
            {
                scrollView.Dispose();
            }

            RecyclePages.Clear();
            VisiblePages.Clear();

            if (ScrollView != null)
            {
                ScrollView.Scrolled -= HandleScrolled;
                ScrollView.RemoveFromSuperview();
                ScrollView.Dispose();
                ScrollView = null;
            }
        }

        #endregion
    }
}

