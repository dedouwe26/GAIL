import sys
import os
import subprocess

def main(args):
    print("======= GAIL BUILDTOOL =======")

    vulkanSDKPath = os.getenv("VULKAN_SDK") if os.getenv("VULKAN_SDK")!=None else os.getenv("VK_SDK_PATH") if os.getenv("VK_SDK_PATH")!=None else None
    if vulkanSDKPath == None:
        print("Vulkan SDK not installed, install via https://vulkan.lunarg.com")
        print("============= END ============")
        exit(1)

    if not os.path.isfile(os.path.dirname(os.path.realpath(__file__))+"/../../compilerpath"):
        print("Please create a file in the tool folder called compilerpath.\nThat file must contain a path to a g++ (GNU) compiler.")
        print("============= END ============")
        exit(1)

    compilerpath = ""
    with open(os.path.dirname(os.path.realpath(__file__))+"/../../compilerpath", "r") as cpathfile:
        compilerpath = cpathfile.readline().strip("\n")

    if not os.path.isdir(compilerpath):
        print("No g++ (GNU) compiler found in that directory.")
        print("============= END ============")
        exit(1)

    if len(args)!=2:
        print("Pass only one argument: (The example name)")
        print("============= END ============")
        exit(1)

    parentExamplePath=os.path.abspath(os.path.join(os.path.dirname(os.path.realpath(__file__)), os.pardir))
    examplePath = parentExamplePath+"/"+args[1]
    GAILPath=os.path.abspath(parentExamplePath+"/../GAIL")

    if not os.path.isdir(examplePath):
        print(f"No such example called: {args[1]}.")
        print("============= END ============")
        exit(1)

    if not os.path.isdir(examplePath+"/bin"):
        os.mkdir(examplePath+"/bin")

    print("Creating executable file (C++).\n")
    print("------- G++ -------")
    print(" ".join([compilerpath+"/g++", examplePath+"/*.cpp", "-Wall", GAILPath+"/bin/GAIL.a", "-I"+vulkanSDKPath+"/Include", "-I"+GAILPath, "-I"+GAILPath+"/include", "-I"+examplePath, "-o"+examplePath+"/bin/main.exe"]))
    gppproc = subprocess.run([compilerpath+"/g++", examplePath+"/*.cpp", "-Wall", GAILPath+"/bin/GAIL.a", "-I"+vulkanSDKPath+"/Include", "-I"+GAILPath, "-I"+GAILPath+"/include", "-I"+examplePath, "-o"+examplePath+"/bin/main.exe"], cwd=compilerpath)
    print("-------------------\n")
        
    print("Build location: examples/C++/"+args[1]+"/bin/main.exe")
    print("Build Successful" if gppproc.returncode==0 else "Build Failed")
    print("========== END BUILD =========\n")
    if not gppproc.returncode == 0:
        exit(1)
    print("Running Example executable...")
    print("\n----- Example -----\n")
    subprocess.run([examplePath+"/bin/main.exe"], cwd=examplePath)
    print("\n-------------------\n")
    print("============= END ============")

if __name__ == "__main__":
    main(sys.argv)