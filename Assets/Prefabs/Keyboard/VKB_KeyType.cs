using System;

public enum VKB_KeyType
{
    // Alphabet keys
    A = 0x41,
    B = 0x42,
    C = 0x43,
    D = 0x44,
    E = 0x45,
    F = 0x46,
    G = 0x47,
    H = 0x48,
    I = 0x49,
    J = 0x4A,
    K = 0x4B,
    L = 0x4C,
    M = 0x4D,
    N = 0x4E,
    O = 0x4F,
    P = 0x50,
    Q = 0x51,
    R = 0x52,
    S = 0x53,
    T = 0x54,
    U = 0x55,
    V = 0x56,
    W = 0x57,
    X = 0x58,
    Y = 0x59,
    Z = 0x5A,

    // Numeric keys
    Key_0 = 0x30,
    Key_1 = 0x31,
    Key_2 = 0x32,
    Key_3 = 0x33,
    Key_4 = 0x34,
    Key_5 = 0x35,
    Key_6 = 0x36,
    Key_7 = 0x37,
    Key_8 = 0x38,
    Key_9 = 0x39,

    // Special keys
    Enter = 0x13, 
    Spacebar = 0x20,
    Shift = 0x10,
    Backspace = 0x08,

    // Symbols and punctuation
    Tilde = 0xC0,         // ~ (tilde)
    Minus = 0xBD,         // - (minus)
    Equal = 0xBB,         // = (equals)
    Slash = 0xBF,         // / (forward slash)
    QuestionMark = 0xBF,  // ? (requires Shift + Slash)
    
    LessThan = 0xBC,      // < (less than, comma key)
    GreaterThan = 0xBE,   // > (greater than, period key)

    LeftBracket = 0xDB,   // [ (left square bracket)
    RightBracket = 0xDD,  // ] (right square bracket)
    
    Semicolon = 0xBA,     // ; (semicolon)
    
    Quote = 0xDE,          // ' or " (single/double quote depending on Shift key)

    // Arrow keys
    Left = 0x25,          // ← (left arrow)
    Right = 0x27,         // → (right arrow)
    
    // Max arrow keys
    MaxLeft = 0x26,       // Max to the left
    MaxRight = 0x28       // Max to the right
}
