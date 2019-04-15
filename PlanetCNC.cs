using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VEMO_PlanetCNC
{
	public class AxisValues
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }
		public double A { get; set; }
		public double B { get; set; }
		public double C { get; set; }
		public double U { get; set; }
		public double V { get; set; }
		public double W { get; set; }

		public override string ToString() => string.Format(
			CultureInfo.InvariantCulture,
			"X:{0} Y:{1} Z:{2} A:{3} B:{4} C:{5} U:{6} V:{7} W:{8}",
			X, Y, Z, A, B, C, U, V, W);
	}

	public class PlanetCNC
	{
		private abstract class Message {
			public abstract void Send(Stream stream, byte[] buffer);
		}

		private class Message<TResult> : Message
		{
			public string Request { get; set; }
			public TaskCompletionSource<TResult> Tcs { get; set; }
			public Func<string, TResult> Converter { get; set; }

			public override void Send(Stream stream, byte[] buffer)
			{
				try
				{
					var requestBytesLength = Encoding.ASCII.GetBytes(Request, 0, Request.Length, buffer, 0);
					buffer[requestBytesLength++] = (byte)'\n';
					stream.Write(buffer, 0, requestBytesLength);
					stream.Flush();

					if (Converter == null)
					{
						Tcs.SetResult(default(TResult));
					}
					else
					{
						var responseBytesLength = stream.Read(buffer, 0, buffer.Length);
						var response = Encoding.ASCII.GetString(buffer, 0, responseBytesLength);

						Tcs.SetResult(Converter(response));
					}
				}
				catch (Exception ex)
				{
					Tcs.SetException(ex);
				}
			}
		}

		private NamedPipeClientStream _pipe;
		private AutoResetEvent _event;
		private Queue<Message> _messages;
		private Thread _thread;
		private bool _run = true;

		public PlanetCNC()
		{
			_pipe = new NamedPipeClientStream(".", "PlanetCNC", PipeDirection.InOut, PipeOptions.None, System.Security.Principal.TokenImpersonationLevel.Impersonation);
			_pipe.Connect(200);

			_event = new AutoResetEvent(false);
			_messages = new Queue<Message>();
			_thread = new Thread(MessageLoop);
			_thread.IsBackground = true;
			_thread.Start();
		}

		private Task<TResult> QueueMessage<TResult>(string request, Func<string, TResult> converter)
		{
			var tcs = new TaskCompletionSource<TResult>() { };

			lock (_messages)
			{
				_messages.Enqueue(new Message<TResult>
				{
					Request = request,
					Tcs = tcs,
					Converter = converter
				});
			}

			_event.Set();

			return tcs.Task;
		}

		private Task QueueMessage(string request) => QueueMessage<bool>(request, null);

		private Message GetMessage()
		{
			lock (_messages)
			{
				if (_messages.Count > 0)
				{
					return _messages.Dequeue();
				}
				else
				{
					return null;
				}
			}
		}

		private void MessageLoop()
		{
			var buffer = new byte[1024];

			while (_run)
			{
				_event.WaitOne();

				Message message;

				while ((message = GetMessage()) != null)
				{
					message.Send(_pipe, buffer);			
				}
			}

			_pipe.Close();
		}

		public void Terminate()
		{
			_run = false;
			_event.Set();
			_thread.Join();
		}

		private double ParseDouble(string s)
		{
			return double.Parse(s, CultureInfo.InvariantCulture);
		}

		private int ParseInt(string s)
		{
			return int.Parse(s, CultureInfo.InvariantCulture);
		}

		private bool ParseBool(string s)
		{
			return s == "1";
		}

		private AxisValues ParseAxisValues(string s)
		{
			var values = new AxisValues();

			foreach (var line in s.Split('\n'))
			{
				var tokens = line.Split(':');

				if (tokens.Length != 2)
				{
					continue;
				}

				var value = ParseDouble(tokens[1]);

				switch (tokens[0]) {
					case "X": values.X = value; break;
					case "Y": values.Y = value; break;
					case "Z": values.Z = value; break;
					case "A": values.A = value; break;
					case "B": values.B = value; break;
					case "C": values.C = value; break;
					case "U": values.U = value; break;
					case "V": values.V = value; break;
					case "W": values.W = value; break;
				}
			}

			return values;
		}

		private bool[] ParseBoolArray(string s)
		{
			var values = new bool[s.Length];
			for (int i = 0; i < s.Length; i++)
			{
				values[i] = s[i] == '1';
			}
			return values;
		}


		/// <summary>
		/// executes mdi command
		/// </summary>
		public Task Mdi(string command) => QueueMessage($"mdi \"{command}\"");

		/// <summary>
		/// get integer parameter value
		/// </summary>
		public Task<int> GetIntParam(string name) => QueueMessage($"param {name}", ParseInt);

		/// <summary>
		/// get boolean parameter value
		/// </summary>
		public Task<bool> GetBoolParam(string name) => QueueMessage($"param {name}", ParseBool);

		/// <summary>
		/// get double parameter value
		/// </summary>
		public Task<double> GetDoubleParam(string name) => QueueMessage($"param {name}", ParseDouble);

		/// <summary>
		/// get string parameter value
		/// </summary>
		public Task<string> GetStringParam(string name) => QueueMessage($"param {name}", s => s);

		/// <summary>
		/// set integer parameter value
		/// </summary>
		public Task SetIntParam(string name, int value) => QueueMessage(string.Format(CultureInfo.InvariantCulture, "param {0}={1}", name, value));

		/// <summary>
		/// set boolean parameter value
		/// </summary>
		public Task SetBoolParam(string name, bool value) => QueueMessage(string.Format("param {0}={1}", name, value ? "1" : "0"));

		/// <summary>
		/// set double parameter value
		/// </summary>
		public Task SetDoubleParam(string name, double value) => QueueMessage(string.Format(CultureInfo.InvariantCulture, "param {0}={1}", name, value));

		/// <summary>
		/// set string parameter value
		/// </summary>
		public Task SetStringParam(string name, string value) => QueueMessage(string.Format("param {0}={1}", name, value));

		/// <summary>
		/// show main window
		/// </summary>
		public Task Show() => QueueMessage("show");

		/// <summary>
		/// hide main window
		/// </summary>
		public Task Hide() => QueueMessage("hide");

		/// <summary>
		/// exits program
		/// </summary>
		public Task Exit() => QueueMessage("exit");

		/// <summary>
		/// toggles EStop
		/// </summary>
		public Task EStop() => QueueMessage("estop");

		/// <summary>
		/// performs Start
		/// </summary>
		public Task Start() => QueueMessage("start");

		/// <summary>
		/// performs Stop
		/// </summary>
		public Task Stop() => QueueMessage("stop");

		/// <summary>
		/// performs Pause
		/// </summary>
		public Task Pause() => QueueMessage("pause");

		/// <summary>
		/// returns emergency stop state
		/// </summary>
		public Task<bool> IsEStop() => QueueMessage("isestop", ParseBool);

		/// <summary>
		/// returns pause state
		/// </summary>
		public Task<bool> IsPause() => QueueMessage("ispause", ParseBool);

		/// <summary>
		/// closes opened GCode
		/// </summary>
		public Task Close() => QueueMessage("close");

		/// <summary>
		/// returns absolute position for all axes
		/// </summary>
		public Task<AxisValues> PosAbs() => QueueMessage("posabs", ParseAxisValues);

		/// <summary>
		/// returns x absolute position
		/// </summary>
		public Task<double> PosAbsX() => QueueMessage("posabsx", ParseDouble);

		/// <summary>
		/// returns y absolute position
		/// </summary>
		public Task<double> PosAbsY() => QueueMessage("posabsy", ParseDouble);

		/// <summary>
		/// returns z absolute position
		/// </summary>
		public Task<double> PosAbsZ() => QueueMessage("posabsz", ParseDouble);

		/// <summary>
		/// returns a absolute position
		/// </summary>
		public Task<double> PosAbsA() => QueueMessage("posabsa", ParseDouble);

		/// <summary>
		/// returns b absolute position
		/// </summary>
		public Task<double> PosAbsB() => QueueMessage("posabsb", ParseDouble);

		/// <summary>
		/// returns c absolute position
		/// </summary>
		public Task<double> PosAbsC() => QueueMessage("posabsc", ParseDouble);

		/// <summary>
		/// returns u absolute position
		/// </summary>
		public Task<double> PosAbsU() => QueueMessage("posabsu", ParseDouble);

		/// <summary>
		/// returns v absolute position
		/// </summary>
		public Task<double> PosAbsV() => QueueMessage("posabsv", ParseDouble);

		/// <summary>
		/// returns w absolute position
		/// </summary>
		public Task<double> PosAbsW() => QueueMessage("posabsw", ParseDouble);

		/// <summary>
		/// returns position for all axes
		/// </summary>
		public Task<AxisValues> Pos() => QueueMessage("pos", ParseAxisValues);

		/// <summary>
		/// returns x position
		/// </summary>
		public Task<double> PosX() => QueueMessage("posx", ParseDouble);

		/// <summary>
		/// returns y position
		/// </summary>
		public Task<double> PosY() => QueueMessage("posy", ParseDouble);

		/// <summary>
		/// returns z position
		/// </summary>
		public Task<double> PosZ() => QueueMessage("posz", ParseDouble);

		/// <summary>
		/// returns a position
		/// </summary>
		public Task<double> PosA() => QueueMessage("posa", ParseDouble);

		/// <summary>
		/// returns b position
		/// </summary>
		public Task<double> PosB() => QueueMessage("posb", ParseDouble);

		/// <summary>
		/// returns c position
		/// </summary>
		public Task<double> PosC() => QueueMessage("posc", ParseDouble);

		/// <summary>
		/// returns u position
		/// </summary>
		public Task<double> PosU() => QueueMessage("posu", ParseDouble);

		/// <summary>
		/// returns v position
		/// </summary>
		public Task<double> PosV() => QueueMessage("posv", ParseDouble);

		/// <summary>
		/// returns w position
		/// </summary>
		public Task<double> PosW() => QueueMessage("posw", ParseDouble);

		/// <summary>
		/// returns speed
		/// </summary>
		public Task<double> Speed() => QueueMessage("speed", ParseDouble);

		/// <summary>
		/// returns accel
		/// </summary>
		public Task<double> Accel() => QueueMessage("accel", ParseDouble);

		/// <summary>
		/// returns spindle speed
		/// </summary>
		public Task<double> Spindle() => QueueMessage("spindle", ParseDouble);

		/// <summary>
		/// returns state for all inputs
		/// </summary>
		public Task<bool[]> Input() => QueueMessage("input", ParseBoolArray);

		/// <summary>
		/// returns input 1 state
		/// </summary>
		public Task<bool> Input1() => QueueMessage("input1", ParseBool);

		/// <summary>
		/// returns input 2 state
		/// </summary>
		public Task<bool> Input2() => QueueMessage("input2", ParseBool);

		/// <summary>
		/// returns input 3 state
		/// </summary>
		public Task<bool> Input3() => QueueMessage("input3", ParseBool);

		/// <summary>
		/// returns input 4 state
		/// </summary>
		public Task<bool> Input4() => QueueMessage("input4", ParseBool);

		/// <summary>
		/// returns input 5 state
		/// </summary>
		public Task<bool> Input5() => QueueMessage("input5", ParseBool);

		/// <summary>
		/// returns input 6 state
		/// </summary>
		public Task<bool> Input6() => QueueMessage("input6", ParseBool);

		/// <summary>
		/// returns input 7 state
		/// </summary>
		public Task<bool> Input7() => QueueMessage("input7", ParseBool);

		/// <summary>
		/// returns input 8 state
		/// </summary>
		public Task<bool> Input8() => QueueMessage("input8", ParseBool);

		/// <summary>
		/// returns state for all limits
		/// </summary>
		public Task<bool[]> Limit() => QueueMessage("limit", ParseBoolArray);

		/// <summary>
		/// returns limit 1 state
		/// </summary>
		public Task<bool> Limit1() => QueueMessage("limit1", ParseBool);

		/// <summary>
		/// returns limit 2 state
		/// </summary>
		public Task<bool> Limit2() => QueueMessage("limit2", ParseBool);

		/// <summary>
		/// returns limit 3 state
		/// </summary>
		public Task<bool> Limit3() => QueueMessage("limit3", ParseBool);

		/// <summary>
		/// returns limit 4 state
		/// </summary>
		public Task<bool> Limit4() => QueueMessage("limit4", ParseBool);

		/// <summary>
		/// returns limit 5 state
		/// </summary>
		public Task<bool> Limit5() => QueueMessage("limit5", ParseBool);

		/// <summary>
		/// returns limit 6 state
		/// </summary>
		public Task<bool> Limit6() => QueueMessage("limit6", ParseBool);

		/// <summary>
		/// returns limit 7 state
		/// </summary>
		public Task<bool> Limit7() => QueueMessage("limit7", ParseBool);

		/// <summary>
		/// returns limit 8 state
		/// </summary>
		public Task<bool> Limit8() => QueueMessage("limit8", ParseBool);

		/// <summary>
		/// returns limit 9 state
		/// </summary>
		public Task<bool> Limit9() => QueueMessage("limit9", ParseBool);

		/// <summary>
		/// returns state for all outputs
		/// </summary>
		public Task<bool[]> Output() => QueueMessage("output", ParseBoolArray);

		/// <summary>
		/// returns output 1 state
		/// </summary>
		public Task<bool> Output1() => QueueMessage("output1", ParseBool);

		/// <summary>
		/// returns output 2 state
		/// </summary>
		public Task<bool> Output2() => QueueMessage("output2", ParseBool);

		/// <summary>
		/// returns output 3 state
		/// </summary>
		public Task<bool> Output3() => QueueMessage("output3", ParseBool);

		/// <summary>
		/// returns output 4 state
		/// </summary>
		public Task<bool> Output4() => QueueMessage("output4", ParseBool);

		/// <summary>
		/// returns output 5 state
		/// </summary>
		public Task<bool> Output5() => QueueMessage("output5", ParseBool);

		/// <summary>
		/// returns output 6 state
		/// </summary>
		public Task<bool> Output6() => QueueMessage("output6", ParseBool);

		/// <summary>
		/// returns output 7 state
		/// </summary>
		public Task<bool> Output7() => QueueMessage("output7", ParseBool);

		/// <summary>
		/// returns output 8 state
		/// </summary>
		public Task<bool> Output8() => QueueMessage("output8", ParseBool);

		/// <summary>
		/// returns aux 1 state
		/// </summary>
		public Task<bool> Aux1() => QueueMessage("aux1", ParseBool);

		/// <summary>
		/// returns aux 2 state
		/// </summary>
		public Task<bool> Aux2() => QueueMessage("aux2", ParseBool);

		/// <summary>
		/// returns aux 3 state
		/// </summary>
		public Task<bool> Aux3() => QueueMessage("aux3", ParseBool);

		/// <summary>
		/// returns aux 4 state
		/// </summary>
		public Task<bool> Aux4() => QueueMessage("aux4", ParseBool);

		/// <summary>
		/// returns amount of free buffer
		/// </summary>
		public Task<int> Buffer() => QueueMessage("buffer", ParseInt);

		/// <summary>
		/// returns percent of buffer utilization
		/// </summary>
		public Task<double> BufferPercent() => QueueMessage("bufferpercent", ParseDouble);

		/// <summary>
		/// returns number of lines in GCode program
		/// </summary>
		public Task<int> LineCount() => QueueMessage("linecount", ParseInt);

		/// <summary>
		/// returns line number currently executing
		/// </summary>
		public Task<int> Line() => QueueMessage("line", ParseInt);

		/// <summary>
		/// opens GCode from file
		/// </summary>
		public Task Open(string filename) => QueueMessage($"open \"{filename}\"");

		/// <summary>
		/// opens GCode from string
		/// </summary>
		public Task GCode(string program) => QueueMessage($"gcode \"{program}\"");

		/// <summary>
		/// returns all JOG values
		/// </summary>
		public Task<bool[]> Jog() => QueueMessage("jog", ParseBoolArray);

		/// <summary>
		/// returns value of JOG 1 pin
		/// </summary>
		public Task<bool> Jog1() => QueueMessage("jog1", ParseBool);

		/// <summary>
		/// returns value of JOG 2 pin
		/// </summary>
		public Task<bool> Jog2() => QueueMessage("jog2", ParseBool);

		/// <summary>
		/// returns value of JOG 3 pin
		/// </summary>
		public Task<bool> Jog3() => QueueMessage("jog3", ParseBool);

		/// <summary>
		/// returns value of JOG 4 pin
		/// </summary>
		public Task<bool> Jog4() => QueueMessage("jog4", ParseBool);

		/// <summary>
		/// returns value of JOG 5 pin
		/// </summary>
		public Task<bool> Jog5() => QueueMessage("jog5", ParseBool);

		/// <summary>
		/// returns value of JOG 6 pin
		/// </summary>
		public Task<bool> Jog6() => QueueMessage("jog6", ParseBool);

		/// <summary>
		/// returns value of JOG 7 pin
		/// </summary>
		public Task<bool> Jog7() => QueueMessage("jog7", ParseBool);

		/// <summary>
		/// returns value of JOG 8 pin
		/// </summary>
		public Task<bool> Jog8() => QueueMessage("jog8", ParseBool);

		/// <summary>
		/// returns value of JOG A1 pin
		/// </summary>
		public Task<bool> JogA1() => QueueMessage("joga1", ParseBool);

		/// <summary>
		/// returns value of JOG A2 pin
		/// </summary>
		public Task<bool> JogA2() => QueueMessage("joga2", ParseBool);

		/// <summary>
		/// returns value of JOG S pin
		/// </summary>
		public Task<bool> JogS() => QueueMessage("jogs", ParseBool);

		/// <summary>
		/// returns value of JOGPOT
		/// </summary>
		public Task<double> JogPot() => QueueMessage("jogpot", ParseDouble);
	}
}
