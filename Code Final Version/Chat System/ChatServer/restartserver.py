import subprocess, psutil, time, sys
from optparse import OptionParser


def main():
	parser = OptionParser()

	parser.add_option("-p","--pid", type="string",dest="pid",help="The caller's process id")
	parser.add_option("-s","--source", type="string", dest="source", help="The full file path to the source file to run in python once the pid is closed.")
	options, args = parser.parse_args()
	
	while int(options.pid) in psutil.get_pid_list():
		pass
	
	time.sleep(2)

	subprocess.Popen(['python',options.source])

if __name__ == "__main__":
	main()