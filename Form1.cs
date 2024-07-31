namespace mouse_events;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void Panel1_MouseWheel(object sender, MouseEventArgs e)
    {
        this.textBox1.AppendText($"{DateTime.Now.ToString("HH:mm:ss.fff"),-15}{e.Delta,-4}{Environment.NewLine}");
    }
}
