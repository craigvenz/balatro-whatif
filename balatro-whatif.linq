<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

Util.HtmlHead.AddStyles(
"""
body { 
	background-color: #253227;
}
.scores {
	background-color: #1d2a2b;
    color: #fdfffd
}
a:link, a:visited {
	color: #66C0F4
}
.chips {
	background-color: #008ffd;
    color: #fdfffd
}
.mult {
	background-color: #fc4a3f;
    color: #fdfffd
}
""");
TextBox TextBoxWithClass(string css) {
    var x = new TextBox();
    x.CssClass = css;
    return x;
}
var chips = TextBoxWithClass("chips");
var mult = TextBoxWithClass("mult");
var roundScore = TextBoxWithClass("scores");
var blindTarget = TextBoxWithClass("scores");
var deficit = new Label();
new StackPanel(true, new Label("Round score:"), roundScore, new Label("Blind Target:"), blindTarget, deficit).Dump();
new StackPanel(true, new Label("Chips:"), chips, new Label("Mult:"), mult).Dump();
var result = new Label("0");
result.Styles["color"] = "#fdfffd";
new StackPanel(true, new Label("Score total for theoretical hand:"), result).Dump();
chips.TextInput += Calculate;
mult.TextInput += Calculate;
roundScore.TextInput += Calculate;
blindTarget.TextInput += Calculate;
var d = Util.LoadString("balatro");
if (!string.IsNullOrEmpty(d))
    (chips.Text, mult.Text, roundScore.Text, blindTarget.Text) = JsonConvert.DeserializeObject<Data>(d)!;
Calculate();
void Calculate(object? sender = null, EventArgs? e = null)
{
    IEnumerable<decimal> ParseString(string txt) => 
        Regex.Split(txt, "\\s+")
             .Where(x => !string.IsNullOrEmpty(x))
             .Select(Parse);
    decimal Sum(string txt) => ParseString(txt).Sum();
    decimal Multiplier(string txt)
    {
        var pieces = Regex.Split(txt, "\\s+").Where(x => !string.IsNullOrEmpty(x));
        var mult = 0m;
        foreach (var item in pieces)
        {
            var m = Regex.Match(item,@"[xX](\d+(\.\d+)?)");
            if (m.Success)
                mult *= Parse(m.Groups[1].Value);
            else
                mult += Parse(item);
        }
        return mult;
    }
    decimal Parse(string txt)
    {
        decimal.TryParse(txt,out var i);
        return Math.Abs(i);
    }
    Util.SaveString("balatro", JsonConvert.SerializeObject(
        new Data(chips.Text,mult.Text,roundScore.Text,blindTarget.Text)));
    var (totalChips, totalMult) = (Sum(chips.Text), Multiplier(mult.Text));
    var pretend = totalChips * totalMult;
    result.Text = $"{totalChips:n0} * {totalMult:n0} = {pretend:n0}";
    var diff = Parse(blindTarget.Text) - (Parse(roundScore.Text) + pretend);
    deficit.Text = $"{Math.Abs(diff):n0}";
    deficit.Styles["color"] = diff > 0 ? "Red" : "Green";
}
record Data(string chips, string mult, string score, string target);
