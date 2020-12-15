using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChainStoreTRPZ2Edition.Helpers
{
    public class UnsignedDoubleBox : TextBox
    {
        public UnsignedDoubleBox()
        {
            PreviewTextInput += DefaultPreviewTextInput;
            DataObject.AddPastingHandler(this, DefaultTextBoxPasting);
        }

        private bool IsTextAllowed(TextBox textBox, string text)
        {
            var newText = textBox.Text.Insert(textBox.CaretIndex, text);
            return double.TryParse(newText, out var res) && res >= 0;
        }

        private void DefaultTextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string) e.DataObject.GetData(typeof(string));

                if (!IsTextAllowed((TextBox) sender, text)) e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void DefaultPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed((TextBox) sender, e.Text);
        }
    }
}