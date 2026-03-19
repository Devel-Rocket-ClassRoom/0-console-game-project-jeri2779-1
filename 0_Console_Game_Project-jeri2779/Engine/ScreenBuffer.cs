using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Framework.Engine
{
    public class ScreenBuffer // 콘솔 화면 버퍼 관리 클래스
    {
        private readonly int _width;                // 버퍼의 너비
        private readonly int _height;               // 버퍼의 높이
        private char[,] _chars;                     // 문자 버퍼
        private ConsoleColor[,] _fgColors;          // 전경색 버퍼
        private ConsoleColor[,] _bgColors;          // 배경색 버퍼
        private readonly StringBuilder _frameBuilder;// 프레임 빌더 (출력 문자열 생성용)

        private static readonly int[] s_ansiFg = { 30, 34, 32, 36, 31, 35, 33, 37, 90, 94, 92, 96, 91, 95, 93, 97 };
        private static readonly int[] s_ansiBg = { 40, 44, 42, 46, 41, 45, 43, 47, 100, 104, 102, 106, 101, 105, 103, 107 };

        public int Width => _width;     // 버퍼의 너비 반환
        public int Height => _height;   // 버퍼의 높이 반환

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);              // 표준 출력 핸들 가져오기

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr handle, out uint mode);// 콘솔 모드 가져오기

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr handle, uint mode);    // 콘솔 모드 설정하기

        public ScreenBuffer(int width, int height)
        {
            _width = width;
            _height = height;
            _chars = new char[height, width];//
            _fgColors = new ConsoleColor[height, width];// 버퍼 초기화
            _bgColors = new ConsoleColor[height, width];// 버퍼 초기화
            _frameBuilder = new StringBuilder(width * height * 4);             // 초기 프레임 빌더 용량 설정
            Clear();
            EnableVirtualTerminalProcessing();
        }

        private static void EnableVirtualTerminalProcessing()
        {
            try
            {
                const int STD_OUTPUT_HANDLE = -11;
                const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

                IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
                if (GetConsoleMode(handle, out uint mode))
                {
                    SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
                }
            }
            catch
            {
                // Non-Windows or unsupported — ANSI may already work
            }
        }

        public void Clear()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _chars[y, x] = ' ';
                    _fgColors[y, x] = ConsoleColor.Gray;
                    _bgColors[y, x] = ConsoleColor.Black;
                }
            }
        }

        public void SetCell(int x, int y, char ch, ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            if (x >= 0 && x < _width && y >= 0 && y < _height)
            {
                _chars[y, x] = ch;
                _fgColors[y, x] = color;
                _bgColors[y, x] = bgColor;
            }
        }

        public void WriteText(int x, int y, string text, ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            for (int i = 0; i < text.Length; i++)
            {
                SetCell(x + i, y, text[i], color, bgColor);
            }
        }

        public void WriteTextCentered(int y, string text, ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            int x = (_width - text.Length) / 2;
            WriteText(x, y, text, color, bgColor);
        }

        public void WriteLines(int x, int y, string[] lines, ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                WriteText(x, y + i, lines[i], color, bgColor);
            }
        }

        public void DrawHLine(int x, int y, int length, char ch = '-', ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            for (int i = 0; i < length; i++)
            {
                SetCell(x + i, y, ch, color, bgColor);
            }
        }

        public void DrawVLine(int x, int y, int length, char ch = '|', ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            for (int i = 0; i < length; i++)
            {
                SetCell(x, y + i, ch, color, bgColor);
            }
        }

        public void DrawBox(int x, int y, int width, int height, ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            SetCell(x, y, '+', color, bgColor);
            SetCell(x + width - 1, y, '+', color, bgColor);
            SetCell(x, y + height - 1, '+', color, bgColor);
            SetCell(x + width - 1, y + height - 1, '+', color, bgColor);

            DrawHLine(x + 1, y, width - 2, '-', color, bgColor);
            DrawHLine(x + 1, y + height - 1, width - 2, '-', color, bgColor);
            DrawVLine(x, y + 1, height - 2, '|', color, bgColor);
            DrawVLine(x + width - 1, y + 1, height - 2, '|', color, bgColor);
        }

        public void FillRect(int x, int y, int width, int height, char ch = ' ', ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    SetCell(x + col, y + row, ch, color, bgColor);
                }
            }
        }

        public void Present()                                   // 버퍼 내용을 콘솔에 출력
        {
            _frameBuilder.Clear();
            _frameBuilder.Append("\x1b[H");

            ConsoleColor currentFg = (ConsoleColor)(-1);
            ConsoleColor currentBg = (ConsoleColor)(-1);

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    ConsoleColor fg = _fgColors[y, x];
                    ConsoleColor bg = _bgColors[y, x];

                    if (fg != currentFg || bg != currentBg)
                    {
                        _frameBuilder.Append("\x1b[");
                        _frameBuilder.Append(s_ansiFg[(int)fg]);
                        _frameBuilder.Append(';');
                        _frameBuilder.Append(s_ansiBg[(int)bg]);
                        _frameBuilder.Append('m');
                        currentFg = fg;
                        currentBg = bg;
                    }

                    _frameBuilder.Append(_chars[y, x]);
                }

                if (y < _height - 1)
                {
                    _frameBuilder.Append('\n');
                }
            }

            _frameBuilder.Append("\x1b[0m");
            Console.Write(_frameBuilder.ToString());
        }
    }
}
