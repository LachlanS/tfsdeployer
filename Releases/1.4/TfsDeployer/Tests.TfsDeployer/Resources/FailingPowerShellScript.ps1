"Output this before failing" ;
$zero = 0; # if we use the literal directly in the division PowerShell fails during parsing.
"Then fail: " + (1/$zero) ;
