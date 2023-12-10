<h1 align="center"><img src="https://raw.githubusercontent.com/dedouwe26/GAIL/main/Logo.svg" alt="logo" width="500", href="https://github.com/dedouwe26/GAIL"/> <br/>
<code>Graphics Audio Input Library</code> <br/></h1>
This library is built using Vulkan, OpenAL and GLFW for the graphics, audio and input / window management.<br/>
<ul>
<li>Fast</li>
<li>C++</li>
<li>Expandible</li>
<li>It's well documented in the header files.</li>
<li>GLFW and OpenAL and Vulkan objects are accessible</li>
</ul>
<br/>
Real world implementations (future): <a href="https://github.com/dedouwe26/Dedhub">Dedhub</a> and <a href="https://github.com/dedouwe26/Taired">Taired</a>. <br/><br/>

If you find a bug or if you have a question, please add a new issue in the issues tab.<br/>
And to learn it you can check the examples or the <a href="https://github.com/dedouwe26/GAIL/wiki">wiki</a>

<br/> A alternative for GAIL is <a href="https://www.raylib.com/index.html">Raylib</a>, but this is built on OpenGL.
<h2>Examples</h2>
For starters there are usage examples that show the fundementals to get started:
<ul>
<li>Hello Window (make a window) [<a href="https://github.com/dedouwe26/GAIL/tree/main/examples/C%2B%2B/HelloWindow">C++</a>] [<a href="https://github.com/dedouwe26/GAIL/tree/main/examples/C%23/HelloWindow">C#</a>]</li>
<li>Hello Triangle (draw a triangle) [<a href="https://github.com/dedouwe26/GAIL/tree/main/examples/C%2B%2B/HelloTriangle">C++</a>] [<a href="https://github.com/dedouwe26/GAIL/tree/main/examples/C%23/HelloTriangle">C#</a>]</li>
</ul>
<h2>Building</h2>
(VSCode recommended, all launch and task files are configured) <br/>
Most releases come pre-built, but if you want to build a example project or build it for your<br> platform, then follow these steps:
<ol>
    <li>Install <a href="https://www.python.org/downloads/">Python 3 or later</a>.</li>
    <li>Install the  <a href="https://vulkan.lunarg.com/sdk/home">LunarG Vulkan SDK</a>.</li>
    <li>Install the GNU G++ compiler.</li>
    <li>Create a file in the repo folder and name it <code>compilerpath</code>. <br/>Set the first line to the path to the binaries of the GNU g++ compiler.</li>
    <li>Choose the following: <br/>
        <b>Follow this if you want to build the <u>main</u> project.</b>
            <ol style="list-style-type: lower-alpha;">
                <li>Open up a terminal and navigate to the <code>./GAIL/tool</code><br/>and use the command : <code>python ./tool.py [cpp / dotnet]</code>.</li>
                <li>There should be the output binary: <code>./GAIL/bin/[GAIL.so / GAIL.dll]</code>. <br/> For <b>C++</b> you'll need to compile your code with the .so file. <br/>For <b>C#</b> you'll need to compile GAIL.NET and reference the DLL file.</li>
            </ol>
        <b>Follow this if you want to build an <u>example</u> project.</b>
            <ol style="list-style-type: lower-alpha;">
                <li>Compile the main project first.</li>
                <li>
                For C#, just run the csproj file in the C# example directory.<br/>
                For C++, open up a terminal and navigate to the <code>./examples/C++/tool</code><br/>and use the command : <code>python ./tool.py [examplename]</code>. <br/>And it should automaticly run the executable.</li>
            </ol>
    </li>
</ol>