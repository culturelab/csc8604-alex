<?php
$tasks = scandir("contents/"); //scan directory for contents
foreach($tasks as $task) //
if (substr($task, 0, 1) !==".") //if substring doesnt statr with 0,1 or .
echo $task."\r\n"  //print list of tasks
?>