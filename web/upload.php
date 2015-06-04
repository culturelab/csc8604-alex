<?php 
$task = $_POST["task_title"]; //title of the task
$text = $_POST["text"]; //text to accompany photo upload
$question = $_POST["task_question"]; //question related to task, only upload if task set

if (!file_exists("contents/".$task)) //searching for missing file name
{
  mkdir("contents/".$task); // create folder for task
  file_put_contents("contents/".$task."/.question.txt", $question);
  //move_uploaded_file($_FILES["task_image"]["tmp_name"], $folder."/photo.jpg");
}

$numbers = scandir("contents/".$task);
 
$number = count($numbers); 
$folder = "contents/".$task."/".($number+1); 
  
mkdir($folder); //create folder for task
move_uploaded_file($_FILES["photo"]["tmp_name"], $folder."/photo.jpg"); //upload photo to folder specific to task
file_put_contents($folder."/text.txt", $text); //upload text to photo in folder specific to task
?>