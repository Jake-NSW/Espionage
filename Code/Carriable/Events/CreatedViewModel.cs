﻿using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct CreatedViewModel( CompositedViewModel ViewModel ) : ISignal;