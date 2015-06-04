	<!--scans contents folder for tasks and creates variables-->
<?php
 $tasks=scandir("contents");
 ?>

<!--starts HTML file and stylesheet for website-->
<!DOCTYPE html>
<html lang="en">
	<head>
		<title>Alex and Sam</title>
		<style type="text/css">
			body { background-color: #262626 }
  			.orange { color: #E46E22; }
			.green { color: #70AD47}
			.font { font-family: Helvetica }
			img.info { width: 300px; float: left }
			.tile { font-family: Helvetica; float: left; font-size:1.5cm }
			div.info { float: left; }
			div.task { margin: 0.5cm; padding: 1cm; background-color: #70AD47; width: 200px; height: 200px; float: left }
			.separator { clear: left }
			a { color: #262626 }
		</style>
	</head>
	
	<body>
		<!--TITLE-->
		<h1 class="orange tile">Welcome to the world of Alex and Sam</h1>
		<div class="separator"></div>
		
		<!--information so image floats left and text appears on the right next to image. Image is a link to the 'bear autopsy page'-->
		<a href="BearAutopsy.html"><img class="info" src='alex.jpg' /></a>
		<div class="info"><p class="green font">This project is made up of a teddy bear that is connected to a website. The teddy is filled with a camera, buttons, and touchscreen. These are then connected to a raspberry pi. Students, teachers, and parents are able to make tasks for children to complete on topics that interest them. While these can include mundane schoolwork, we would encourage the individuals to move more towards topics of their interest such as cultural festivals, life at home, or any other topic; these tasks can then be completed by taking a picture and entering text via the teddy bears. All this information will be liable to moderation from a parent or teacher through the use of an RFID tag before it is uploaded to the website where each new added task is turned into a separate tab (see Flowchart in the sidebar). 
We would encourage teachers to use the teddy bears in the classroom as well as outside of the classroom to encourage home-classroom sharing within an open learning environment. As students are able to start and complete any (and as many) tasks they want they are encouraged to venture into their imagination, expanding their learning horizons. <br></br>
Connecting classrooms and students worlds has been a challenge for pedagogy, teachers, schools, parents, and students for a long time. The ideas of opening the classroom and the development of student-curated notes, assignments, and textbooks were developed by Falko Peschel (1969-present) and Celestine Freinet (1896-1966) respectively. We have used the ideas of these two thinkers as well as taking the realities of new media and its effects on learning both in and out of the classroom to develop Alex and Sam. These are two stuffed animals filled with a camera and touchscreen (among other technologies) developed to allow students to co-create and co-curate tasks and learning outcomes mediated by the use of technology in a methodological, content-driven, as well as socially integrated open learning environment. <br>
			
</p></div>
			
		<!--separator is needed to have a blank slate for the rest of the code after this part-->
		<div class="separator"></div></div>
			<?php
  foreach($tasks as $task)  
  if (substr($task, 0, 1) !== ".")  {
  ?>
			<!--task images and descriptions are used as a link to each separate 'task' page-->	
			<a href="task.php?task=<?php echo $task?>"><div class="task font">
				<!--image for task is commented out as this would be a nice feature to have in a future iteration of the design-->
				<!--<img src='<?php echo '/contents' .$task. '/photo.jpg' ?>'></img><br>-->
					<h1><?php echo $task ?></h1>
					<p><?php echo file_get_contents('contents/'.$task. '/.question.txt') ?></p>
			</div></a>
		
			<?php } ?>
	</body>
</html>