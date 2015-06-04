<!--gets all the tasks and creates variable with those tasks-->
<?php
  $task = $_GET["task"]
  
  ?>
<!--starts HTML document and stylesheet-->
<!DOCTYPE html>
<html lang="en">
	<head>
		<title>Alex and Sam: Tasks</title>
		<style type="text/css">
			body { background-color: #262626}
  			.orange { color: #E46E22; }
			.green { color: #70AD47}
			.tile { font-family: Helvetica; float: left; font-size:2cm }
			.heading { font-size: 0.75cm; color:#E46E22; font-family: Helvetica }
			.font { font-family: Helvetica }
			.contributiontext { font-family: Helvetica; float: left; margin: 0.5cm; padding: 0.5cm }
			div.task { margin: 0.5cm; padding: 1cm; background-color: #70AD47; width: 300px; height: 300px; clear: left }
			.separator { clear: left; margin: 0.5cm }
			img.contribution { width: 200px; border-color:  #70AD47; border-style: solid; border-width: 0.5cm; float: left; margin:0.5cm }
			a { color: #70AD47 }
		</style>
	</head>
	
	<body>
		<!--homepage button-->
		<a href="index.php"><p class="green font">Homepage</p></a>
		<!--Site Title-->
		<h1 class="orange font title"><?php echo $task?></h1>
		
		<!--task question-->
		<p class="green font "><?php echo file_get_contents('contents/'.$task. '/.question.txt')?></p>
			
		<h1 class="heading">Contributions</h1>
		
		<!--Contributions: scans contents folder for tasks and makes sure to repeat all following code for all contributions-->
		<?php
  $contributions=scandir('contents/'.$task);
foreach($contributions as $contribution)
  //only contributions that have more than one symbol in the name and does not start with '.' will be portrayed
  if (substr($contribution, 0, 1) !== ".")  
  {?>
		<div class="separator">
			 <img class="contribution" src="<?php echo 'contents/'.$task.'/'.$contribution.'/photo.jpg'?>"/>
				 <div class="green contributiontext"><?php echo file_get_contents('contents/'. $task.'/'.$contribution.'/text.txt')?></div>
		</div>
		<?php }?>
	</body>
</html>