# Coding Style

The primary goals are:

- Make code as simple and safe as possible to refactor and reuse

Some of principles:

These apply to classes and functions 

- Be concise 
- Avoid duplication 
- Minimize side effects
- Code should be easy to reason about and easily understood in isolation
- Code should be discoverable 
- It should be hard to use code wrong 
- minimize dependencies (temporal or state)
- Prefer static functions
- Prefer public function
- Prefer extension functions 
- Prefer readonly fields

Additional Recommendations

- Use LINQ where possible
- Almost always make code public unless it is explicitly better as private
- Prefer functions that return results to out parameters
- Minimize the number of arguments to functions 
- If a field can be readonly make it readonly 
- Prefer short functions to long functions
- Prefer not to use nested function
- Prefer static functions to documented sections of code     
- Use expression properties 
- Use expression method bodies where possible
- Use var generously 
- Try to avoid duplication
- Merge statements aggressively

When Refactoring Code

1. Look at each statement and expression and their context 
2. Ask yourself:
    a. would a static function make this code shorter or simpler to understand
    b. is this a pattern that is likely to occur again 
3. If a static function would help, look for it, or create it 
4. If a type would help, look for it, create it if the benefits are significant, or the     likelihood over time of reusing it is high. 


