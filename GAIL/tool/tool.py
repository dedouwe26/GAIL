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

    if not os.path.isdir(projectPath+"/bin"):
        os.mkdir(projectPath+"/bin")

    print("Creating static library (.a) file.\n")
    print("------- G++ -------")
    objectFiles = []
    returncode = 0
    for cppfile in (glob.glob(projectPath+"/*.cpp")+glob.glob(projectPath+"/base/*.cpp")):
        objectFiles.append(projectPath+"/bin/"+os.path.split(cppfile)[1]+".o")
        gppproc = subprocess.run([compilerpath+"/g++", "-Wall", "-I"+includePath, "-I"+vulkanSDKPath+"/Include", "-I"+projectPath, "-I"+projectPath+"/base", "-L", vulkanSDKPath+"/Lib/vulkan-1.lib", "-L",libPath, "-lglfw3", "-L",libPath+"/OpenAL32.lib", "-c", cppfile, "-o"+projectPath+"/bin/"+os.path.split(cppfile)[1]+".o"], cwd=compilerpath)
        returncode += gppproc.returncode
    arargs=[compilerpath+"/ar", "rvs", projectPath+"/bin/GAIL.a"]
    arargs.extend(objectFiles)
    arproc = subprocess.run(arargs, cwd=compilerpath)
    returncode+=arproc.returncode
    print("-------------------\n")
        
    print("Build location: GAIL/bin/GAIL.a")
    print("Build Successful" if returncode==0 else "Build Failed")
    print("============= END ============")
    exit(0 if returncode==0 else 1)

if __name__ == "__main__":
    main(sys.argv)