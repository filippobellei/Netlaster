using Android.Views;

namespace Netlaster;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        var rootLayout = new RelativeLayout(this)
        {
            LayoutParameters = new RelativeLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent)
        };

        var textView = new TextView(this)
        {
            Text = "Hello Netlaster!",
            TextSize = 24
        };

        var textParams = new RelativeLayout.LayoutParams(
            ViewGroup.LayoutParams.WrapContent,
            ViewGroup.LayoutParams.WrapContent);

        textParams.AddRule(LayoutRules.CenterInParent);
        textView.LayoutParameters = textParams;

        rootLayout.AddView(textView);

        SetContentView(rootLayout);
    }
}
