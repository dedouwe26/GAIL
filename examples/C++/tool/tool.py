import sys
import os
import subprocess

def main(args):
    print("======= GAIL BUILDTOOL =======")

    if not os.path.isfile(os.path.dirname(os.path.realpath(__file__))+"/../../../compilerpath"):
        print("Please create a file in the tool folder called compilerpath.\nThat file must contain a path to a g++ (GNU) compiler.")
        print("============= END ============")
        exit()

    compilerpath = ""
    with open(os.path.dirname(os.path.realpath(__file__))+"/../../../compilerpath", "r") as cpathfile:
        compilerpath = cpathfile.readline().strip("\n")

    if not os.path.isdir(compilerpath):
        print("No g++ (GNU) compiler found in that directory.")
        print("============= END ============")
        exit()

    if len(args)!=2:
        print("Pass only one argument: (The example name)")
        print("============= END ============")
        exit()

    parentExamplePath=os.path.abspath(os.path.join(os.path.dirname(os.path.realpath(__file__)), os.pardir))
    examplePath = parentExamplePath+"/"+args[1]
    GAILPath=os.path.abspath(parentExamplePath+"/../../GAIL")

    if not os.path.isdir(examplePath):
        print(f"No such example called: {args[1]}.")
        print("============= END ============")
        exit()

    if not os.path.isdir(examplePath+"/bin"):
        os.mkdir(examplePath+"/bin")

    print("Creating executable file (C++).\n")
    print("------- G++ -------")
    gppproc = subprocess.run([compilerpath+"/g++", "-I"+GAILPath, "-I"+examplePath, examplePath+"/*.cpp", "-L"+GAILPath+"/bin/GAIL.a", "-o"+examplePath+"/bin/main.exe"], cwd=compilerpath)
    returncode = gppproc.returncode
    print("-------------------\n")
        
    print("Build location: examples/C++/"+args[1]+"/bin/main.exe")
    print("Build Successful" if returncode==0 else "Build Failed")
    print("========== END BUILD =========\n")
    if not returncode == 0:
        exit()
    print("Running Example executable...")
    print("\n----- Example -----\n")
    subprocess.run([examplePath+"/bin/main.exe"], cwd=examplePath)
    print("\n-------------------\n")
    print("============= END ============")

if __name__ == "__main__":
    main(sys.argv)