import sys
import os
import subprocess
import glob

def main(args):
    print("======= GAIL BUILDTOOL =======")

    vulkanSDKPath = os.getenv("VULKAN_SDK") if os.getenv("VULKAN_SDK")!=None else os.getenv("VK_SDK_PATH") if os.getenv("VK_SDK_PATH")!=None else None
    if vulkanSDKPath == None:
        print("Vulkan SDK not installed, install via https://vulkan.lunarg.com")
        print("============= END ============")
        exit()
    
    if not os.path.isfile(os.path.dirname(os.path.realpath(__file__))+"/../../compilerpath"):
        print("Please create a file in the repository folder called compilerpath.\nThat file must contain a path to a g++ (GNU) compiler.")
        print("============= END ============")
        exit()

    compilerpath = ""
    with open(os.path.dirname(os.path.realpath(__file__))+"/../../compilerpath", "r") as cpathfile:
        compilerpath = cpathfile.readline().strip("\n")

    if not os.path.isdir(compilerpath):
        print("No g++ (GNU) compiler found in that directory.")
        print("============= END ============")
        exit()

    projectPath=os.path.abspath(os.path.join(os.path.dirname(os.path.realpath(__file__)), os.pardir))
    includePath=projectPath+"/include"
    libPath=projectPath+"/lib"

    returncode = 1    

    if len(args)!=2:
        print("Pass only one argument: cpp / dotnet")
        print("============= END ============")
        exit()

    if not os.path.isdir(projectPath+"/bin"):
        os.mkdir(projectPath+"/bin")

    if args[1] == "cpp":
        print("Creating lib file (C++).\n")
        print("------- G++ -------")
        objectFiles = []
        returncode = 0
        for cppfile in (glob.glob(projectPath+"/*.cpp")+glob.glob(projectPath+"/base/*.cpp")):
            objectFiles.append(projectPath+"/bin/"+os.path.split(cppfile)[1]+".o")
            gppproc = subprocess.run([compilerpath+"/g++", "-I"+includePath, "-I"+vulkanSDKPath+"/Include", "-I"+projectPath, "-I"+projectPath+"/base", "-L", vulkanSDKPath+"/Lib/vulkan-1.lib", "-L",libPath+"/glfw3.lib", "-c", cppfile, "-o"+projectPath+"/bin/"+os.path.split(cppfile)[1]+".o"], cwd=compilerpath)
            returncode = gppproc.returncode
        arargs=[compilerpath+"/ar", "rvs", projectPath+"/bin/GAIL.a"]
        arargs.extend(objectFiles)
        arproc = subprocess.run(arargs, cwd=compilerpath)
        returncode+=arproc.returncode
        print("-------------------\n")
    elif args[1] == "dotnet":
        print("Creating DLL (for GAIL.NET).\n")
        print("------- G++ -------")
        gppproc = subprocess.run([compilerpath+"/g++", "-I"+includePath, "-I"+vulkanSDKPath+"/Include", "-I"+projectPath,  "-L", vulkanSDKPath+"/Lib/vulkan-1.lib", "-L",libPath+"/glfw3.lib", projectPath+"/*.cpp", projectPath+"/bindings/*.cpp", "-o"+projectPath+"/bin/GAIL.exe"], cwd=compilerpath) # >>> TODO <<< Export to dll file
        returncode = gppproc.returncode
        print("-------------------\n")
    else:
        print("Choose of: cpp / dotnet")
        print("============= END ============")
        exit()
        
    print("Build location: GAIL/bin/GAIL."+("dll" if args[1] == "dotnet" else "a"))
    print("Build Successful" if returncode==0 else "Build Failed")
    print("============= END ============")

if __name__ == "__main__":
    main(sys.argv)