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
.timesMult {
	background-color: #ff4b41;
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
var timesMult = TextBoxWithClass("timesMult");
var roundScore = TextBoxWithClass("scores");
var blindTarget = TextBoxWithClass("scores");
var deficit = new Label();
new StackPanel(true, new Label("Round score:"), roundScore, new Label("Blind Target:"), blindTarget, deficit).Dump();
new StackPanel(true, new Label("Chips:"), chips, new Label("Mult:"), mult, new Label("xMult:"), timesMult).Dump();
var result = new Label("0");
result.Styles["color"] = "#fdfffd";
new StackPanel(true, new Label("Score total for theoretical hand:"), result).Dump();
chips.TextInput += Calculate;
mult.TextInput += Calculate;
timesMult.TextInput += Calculate;
roundScore.TextInput += Calculate;
blindTarget.TextInput += Calculate;
var d = Util.LoadString("balatro");
if (!string.IsNullOrEmpty(d))
    (chips.Text, mult.Text, timesMult.Text, roundScore.Text, blindTarget.Text) = JsonConvert.DeserializeObject<Data>(d)!;
Calculate();
void Calculate(object? sender = null, EventArgs? e = null)
{
    IEnumerable<decimal> ParseString(string txt) => 
        Regex.Split(txt, "\\s*")
             .Where(x => !string.IsNullOrEmpty(x))
             .Select(Parse);
    decimal Sum(string txt) => ParseString(txt).Sum();
    decimal Multiplier(decimal i) => i == 0 ? 1m : i;
    decimal Parse(string txt)
    {
        decimal.TryParse(txt,out var i);
        return Math.Abs(i);
    }
    Util.SaveString("balatro", JsonConvert.SerializeObject(
        new Data(chips.Text,mult.Text,timesMult.Text,roundScore.Text,blindTarget.Text)));
    var pretend = Sum(chips.Text) * Multiplier(Sum(mult.Text));
    foreach (var item in ParseString(timesMult.Text))
        pretend *= Multiplier(item);
    result.Text = $"{pretend:n0}";
    var diff = Parse(blindTarget.Text) - (Parse(roundScore.Text) + pretend);
    deficit.Text = $"{Math.Abs(diff):n0}";
    deficit.Styles["color"] = diff > 0 ? "Red" : "Green";
}
record Data(string chips, string mult, string times, string score, string target);
