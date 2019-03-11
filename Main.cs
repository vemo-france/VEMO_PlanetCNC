using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VEMO_PlanetCNC
{
	public partial class Main : Form
	{
		private readonly PlanetCNC _pcnc;

		public Main()
		{
			InitializeComponent();

			_pcnc = new PlanetCNC();

			AddButton("mdi", async () => await _pcnc.Mdi("/101"));

			AddButton("param", async () => _result.Items.Add("color3DGrid = " + await _pcnc.GetStringParam("color3DGrid")));

			AddButton("show", async () => await _pcnc.Show());
			AddButton("hide", async () => await _pcnc.Hide());
			AddButton("exit", async () => await _pcnc.Exit());
			AddButton("estop", async () => await _pcnc.EStop());
			AddButton("start", async () => await _pcnc.Start());
			AddButton("stop", async () => await _pcnc.Stop());

			AddButton("pause", async () => await _pcnc.Pause());
			AddButton("isestop", async () => _result.Items.Add(await _pcnc.IsEStop()));
			AddButton("ispause", async () => _result.Items.Add(await _pcnc.IsPause()));

			AddButton("close", async () => await _pcnc.Close());

			AddButton("posabs", async () => _result.Items.Add(await _pcnc.PosAbs()));
			AddButton("posabsx", async () => _result.Items.Add(await _pcnc.PosAbsX()));
			AddButton("posabsy", async () => _result.Items.Add(await _pcnc.PosAbsY()));
			AddButton("posabsz", async () => _result.Items.Add(await _pcnc.PosAbsZ()));
			AddButton("posabsa", async () => _result.Items.Add(await _pcnc.PosAbsA()));
			AddButton("posabsb", async () => _result.Items.Add(await _pcnc.PosAbsB()));
			AddButton("posabsc", async () => _result.Items.Add(await _pcnc.PosAbsC()));
			AddButton("posabsu", async () => _result.Items.Add(await _pcnc.PosAbsU()));
			AddButton("posabsv", async () => _result.Items.Add(await _pcnc.PosAbsV()));
			AddButton("posabsw", async () => _result.Items.Add(await _pcnc.PosAbsW()));

			AddButton("pos", async () => _result.Items.Add(await _pcnc.Pos()));
			AddButton("posx", async () => _result.Items.Add(await _pcnc.PosX()));
			AddButton("posy", async () => _result.Items.Add(await _pcnc.PosY()));
			AddButton("posz", async () => _result.Items.Add(await _pcnc.PosZ()));
			AddButton("posa", async () => _result.Items.Add(await _pcnc.PosA()));
			AddButton("posb", async () => _result.Items.Add(await _pcnc.PosB()));
			AddButton("posc", async () => _result.Items.Add(await _pcnc.PosC()));
			AddButton("posu", async () => _result.Items.Add(await _pcnc.PosU()));
			AddButton("posv", async () => _result.Items.Add(await _pcnc.PosV()));
			AddButton("posw", async () => _result.Items.Add(await _pcnc.PosW()));

			AddButton("speed", async () => _result.Items.Add(await _pcnc.Speed()));
			AddButton("accel", async () => _result.Items.Add(await _pcnc.Accel()));
			AddButton("spindle", async () => _result.Items.Add(await _pcnc.Spindle()));

			AddButton("input", async () => _result.Items.Add(string.Join(", ", (await _pcnc.Input()).Select(v => v.ToString()).ToArray())));
			AddButton("input1", async () => _result.Items.Add(await _pcnc.Input1()));
			AddButton("input2", async () => _result.Items.Add(await _pcnc.Input2()));
			AddButton("input3", async () => _result.Items.Add(await _pcnc.Input3()));
			AddButton("input4", async () => _result.Items.Add(await _pcnc.Input4()));
			AddButton("input5", async () => _result.Items.Add(await _pcnc.Input5()));
			AddButton("input6", async () => _result.Items.Add(await _pcnc.Input6()));
			AddButton("input7", async () => _result.Items.Add(await _pcnc.Input7()));
			AddButton("input8", async () => _result.Items.Add(await _pcnc.Input8()));

			AddButton("limit", async () => _result.Items.Add(string.Join(", ", (await _pcnc.Limit()).Select(v => v.ToString()).ToArray())));
			AddButton("limit1", async () => _result.Items.Add(await _pcnc.Limit1()));
			AddButton("limit2", async () => _result.Items.Add(await _pcnc.Limit2()));
			AddButton("limit3", async () => _result.Items.Add(await _pcnc.Limit3()));
			AddButton("limit4", async () => _result.Items.Add(await _pcnc.Limit4()));
			AddButton("limit5", async () => _result.Items.Add(await _pcnc.Limit5()));
			AddButton("limit6", async () => _result.Items.Add(await _pcnc.Limit6()));
			AddButton("limit7", async () => _result.Items.Add(await _pcnc.Limit7()));
			AddButton("limit8", async () => _result.Items.Add(await _pcnc.Limit8()));
			AddButton("limit9", async () => _result.Items.Add(await _pcnc.Limit9()));

			AddButton("output", async () => _result.Items.Add(string.Join(", ", (await _pcnc.Output()).Select(v => v.ToString()).ToArray())));
			AddButton("output1", async () => _result.Items.Add(await _pcnc.Output1()));
			AddButton("output2", async () => _result.Items.Add(await _pcnc.Output2()));
			AddButton("output3", async () => _result.Items.Add(await _pcnc.Output3()));
			AddButton("output4", async () => _result.Items.Add(await _pcnc.Output4()));
			AddButton("output5", async () => _result.Items.Add(await _pcnc.Output5()));
			AddButton("output6", async () => _result.Items.Add(await _pcnc.Output6()));
			AddButton("output7", async () => _result.Items.Add(await _pcnc.Output7()));
			AddButton("output8", async () => _result.Items.Add(await _pcnc.Output8()));

			AddButton("aux1", async () => _result.Items.Add(await _pcnc.Aux1()));
			AddButton("aux2", async () => _result.Items.Add(await _pcnc.Aux2()));
			AddButton("aux3", async () => _result.Items.Add(await _pcnc.Aux3()));
			AddButton("aux4", async () => _result.Items.Add(await _pcnc.Aux4()));

			AddButton("buffer", async () => _result.Items.Add(await _pcnc.Buffer()));
			AddButton("bufferpercent", async () => _result.Items.Add(await _pcnc.BufferPercent()));
			AddButton("linecount", async () => _result.Items.Add(await _pcnc.LineCount()));
			AddButton("line", async () => _result.Items.Add(await _pcnc.Line()));

			AddButton("open", async () => {
				using (var dialog = new OpenFileDialog())
				{
					if (dialog.ShowDialog() == DialogResult.OK)
					{
						await _pcnc.Open(dialog.FileName);
					}
				}
			});

			AddButton("gcode", async () => await _pcnc.GCode("G0 X10 Y10"));

			AddButton("jog", async () => _result.Items.Add(string.Join(", ", (await _pcnc.Jog()).Select(v => v.ToString()).ToArray())));
			AddButton("jog1", async () => _result.Items.Add(await _pcnc.Jog1()));
			AddButton("jog2", async () => _result.Items.Add(await _pcnc.Jog2()));
			AddButton("jog3", async () => _result.Items.Add(await _pcnc.Jog3()));
			AddButton("jog4", async () => _result.Items.Add(await _pcnc.Jog4()));
			AddButton("jog5", async () => _result.Items.Add(await _pcnc.Jog5()));
			AddButton("jog6", async () => _result.Items.Add(await _pcnc.Jog6()));
			AddButton("jog7", async () => _result.Items.Add(await _pcnc.Jog7()));
			AddButton("jog8", async () => _result.Items.Add(await _pcnc.Jog8()));
			AddButton("joga1", async () => _result.Items.Add(await _pcnc.JogA1()));
			AddButton("joga2", async () => _result.Items.Add(await _pcnc.JogA2()));
			AddButton("jogs", async () => _result.Items.Add(await _pcnc.JogS()));
			AddButton("jogpot", async () => _result.Items.Add(await _pcnc.JogPot()));

			LayoutContent();
		}

		protected override void OnClosed(EventArgs e)
		{
			_pcnc.Terminate();
		}

		private void AddButton(string name, Action action)
		{
			var button = new Button()
			{
				Text = name
			};
			button.Click += (s, e) => action();
			_buttonsPanel.Controls.Add(button);
		}

		private void LayoutContent()
		{
			var ButtonHeight = _dummyLabel.Height * 2;
			var ButtonWidth = ButtonHeight * 4;
			var Margin = ButtonHeight / 2;

			var availableHeight = (ClientRectangle.Height - Margin * 3);
			var mid = availableHeight / 2;
			_buttonsPanel.SetBounds(Margin, Margin, ClientRectangle.Width - (Margin * 2), mid);
			_result.SetBounds(Margin, mid + Margin * 2, ClientRectangle.Width - (Margin * 2), availableHeight - mid);

			int x = 0, y = 0;


			foreach (var child in _buttonsPanel.Controls.OfType<Button>().OrderBy(e => e.Text))
			{
				child.SetBounds(x, y, ButtonWidth, ButtonHeight);
				x += ButtonWidth + Margin;

				if (x + ButtonWidth > _buttonsPanel.ClientRectangle.Width)
				{
					x = 0;
					y += ButtonHeight + Margin;
				}
			}

		}

		protected override void OnResize(EventArgs e)
		{
			LayoutContent();
		}

	}
}
