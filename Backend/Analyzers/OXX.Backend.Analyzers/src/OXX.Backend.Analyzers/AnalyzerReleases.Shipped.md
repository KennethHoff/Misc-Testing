## Release 1.0

### New Rules

Rule ID | Category | Severity | Notes                                          
--------|----------|----------|------------------------------------------------
OXX0000 | Design   | Info     | Analyzer error. Contact the analyzer developer.
OXX9001 | Design   | Warning  | Switch expressions on OneOf.Value must be exhaustive.
OXX9002 | Design   | Warning  | Switch expressions on OneOf.Value should not check for literals or impossible cases.
OXX9003 | Design   | Warning  | Switch expressions on OneOf.Value should not include discard patterns.
OXX9005 | Design   | Warning  | Methods should return OneOf<T, Exception> instead of throwing exceptions.
