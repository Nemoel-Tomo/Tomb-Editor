﻿using System.Drawing;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using System.Text.RegularExpressions;

namespace TombLib.Controls
{
    public partial class LuaTextBox : UserControl
    {
        private static TextStyle commentColor = new TextStyle(new SolidBrush(Color.Green), null, FontStyle.Regular);
        private static TextStyle regularColor = new TextStyle(null, null, FontStyle.Regular);
        private static TextStyle operatorsColor = new TextStyle(new SolidBrush(Color.Orange), null, FontStyle.Bold);
        private static TextStyle keywordsColor = new TextStyle(new SolidBrush(Color.CornflowerBlue), null, FontStyle.Bold);

        private static string[] keywords = new string[]
       {
            "if",
            "end",
            "function",
            "for",
            "true",
            "false",
            "return",
            "then"
       };

        private static string[] operators = new string[]
        {
            "==",
            "and",
            "or",
            "~=",
            ":",
            @"\."
        };

        public LuaTextBox()
        {
            InitializeComponent();
        }

        public static void DoSyntaxHighlighting(TextChangedEventArgs e)
        {
            // Clear styles
            e.ChangedRange.ClearStyle(
                    commentColor, regularColor);

            // Apply styles (THE ORDER IS IMPORTANT!)
            e.ChangedRange.SetStyle(commentColor, @"--.*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(regularColor, @"[\[\],]");
            e.ChangedRange.SetStyle(operatorsColor, @"(" + string.Join("|", operators) + @")");
            e.ChangedRange.SetStyle(keywordsColor, @"(" + string.Join("|", keywords) + @")");
        }

        public void Paste(string text) => textEditor.InsertText(text);

        public string Code
        {
            get { return textEditor.Text; }
            set { textEditor.Text = value; }
        }

        private void textEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            DoSyntaxHighlighting(e);
            textEditor.Invalidate();
        }

        private void textEditor_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
            OnDragEnter(e);
        }

        private void textEditor_DragDrop(object sender, DragEventArgs e)
        {
            OnDragDrop(e);
        }
    }
}
