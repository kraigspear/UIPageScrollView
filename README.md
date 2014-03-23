UIPageScrollView
================

Xamarin c# Paging example

public override void ViewDidLoad()
{
    base.ViewDidLoad();
  
    const int NumberOfImages = 14;

    ScrollPageView = new UIPageingScrollView(this, scrollPage =>
    {
        var imageFileName = string.Format("IMG_{0}.jpg", 1185 + scrollPage.Index);
        var image = UIImage.FromFile(imageFileName);

        ViewControllerWithImage viewController;

        if (scrollPage.ViewController == null)
        {
            viewController = (ViewControllerWithImage)Storyboard.InstantiateViewController("imageVC");
            scrollPage.ViewController = viewController;
        }
        else
        {
            viewController = (ViewControllerWithImage)scrollPage.ViewController;
        }

        viewController.Image = image;
    });

    ScrollPageView.NumberOfPages = NumberOfImages;
   
}
