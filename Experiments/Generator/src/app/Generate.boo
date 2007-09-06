namespace Generator

import System

[STAThread]
def Main(argv as (string)):
	return GeneratorFactory.Instance.CreateAndRun(argv)
	
