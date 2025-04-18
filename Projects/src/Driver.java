import java.io.*;
import java.util.*;
import java.util.stream.IntStream;

public class Driver
{

	public static void main(String[] args)
	{
		Scanner scannerInput = new Scanner(System.in);
		Scanner scannerInput2 = new Scanner(System.in);
		BufferedReader br;
//		List<String> myCollection = new ArrayList<>();  //0.129, 0.116, 0.134, 0.136, 0.131 = Avg: 0.129 Seconds
//		List<String> myCollection = new LinkedList<>(); //0.203, 0.200, 0.199, 0.203, 0.201 = Avg: 0.201 Seconds 
		Set<String> myCollection = new HashSet<>();     //0.041, 0.068, 0.043, 0.038, 0.043 = Avg: 0.047 Seconds = Winner!
//		Set<String> myCollection = new TreeSet<>();     //0.056, 0.050, 0.064, 0.080, 0.069 = Avg: 0.064 Seconds
		Map<String, Integer> myMap = new TreeMap<>();
		List<Word> wordList = new ArrayList<>();
		String wordLine;
		
		System.out.println("Welcome, please select a number to count the unique words in the reading!");
		System.out.println("");
		System.out.println("1: Alice In Wonderland");
		System.out.println("2: The Bible (King James Version)");
		System.out.println("3: Frankenstein");
		System.out.println("4: Moby Dick");
		System.out.println("5: Romeo and Juliet");
		System.out.println("6: The Great Gatsby");
		System.out.println("7: A Tale of Two Cities");
		System.out.println("8: Great Expectations");
		System.out.println("");
		System.out.print("Enter Selection: ");
		int userInput = scannerInput.nextInt();
//		System.out.println("User Input is: " + userInput);
		System.out.println("");
		String FileString = "";
		String BookName = "";
		int[] selection = {1, 2, 3, 4, 5, 6, 7, 8};
		boolean found = IntStream.of(selection).anyMatch(n -> n == userInput);
		
		if(found)
		{
			if(userInput == 1)
			{
				FileString = "C:\\Users\\Owner\\Desktop\\CSCI 299 - Thesis & Capstone\\Portfolio\\codycusey.github.io\\Projects\\src\\Alice in Wonderland.txt";
				BookName = "Alice in Wonderland";
			}
			if(userInput == 2)
			{
				FileString = "C:\\Users\\Owner\\Desktop\\CSCI 299 - Thesis & Capstone\\Portfolio\\codycusey.github.io\\Projects\\src\\The Bible KJV.txt";
				BookName = "The Bible (King James Verison)";
			}
			if(userInput == 3)
			{
				FileString = "C:\\Users\\Owner\\Desktop\\CSCI 299 - Thesis & Capstone\\Portfolio\\codycusey.github.io\\Projects\\src\\Frankenstein.txt";
				BookName = "Frankenstein";
			}
			if(userInput == 4)
			{
				FileString = "C:\\Users\\Owner\\Desktop\\CSCI 299 - Thesis & Capstone\\Portfolio\\codycusey.github.io\\Projects\\src\\Moby Dick.txt";
				BookName = "Moby Dick";
			}
			if(userInput == 5)
			{
				FileString = "C:\\Users\\Owner\\Desktop\\CSCI 299 - Thesis & Capstone\\Portfolio\\codycusey.github.io\\Projects\\src\\Romeo and Juliet.txt";
				BookName = "Romeo and Juliet";
			}
			if(userInput == 6)
			{
				FileString = "C:\\Users\\Owner\\Desktop\\CSCI 299 - Thesis & Capstone\\Portfolio\\codycusey.github.io\\Projects\\src\\The Great Gatsby.txt";
				BookName = "The Great Gatsby";
			}
			if(userInput == 7)
			{
				FileString = "C:\\Users\\Owner\\Desktop\\CSCI 299 - Thesis & Capstone\\Portfolio\\codycusey.github.io\\Projects\\src\\A Tale of Two Cities.txt";
				BookName = "A Tale of Two Cities";
			}
			if(userInput == 8)
			{
				FileString = "C:\\Users\\Owner\\Desktop\\CSCI 299 - Thesis & Capstone\\Portfolio\\codycusey.github.io\\Projects\\src\\Great Expectations.txt";
				BookName = "Great Expectations";
			}
		
		long startTime = System.currentTimeMillis();
			
		try
		{
			br = new BufferedReader(new FileReader(FileString));
			String nextLine;
				
			try
			{
				while((nextLine = br.readLine()) != null)
				{
					StringTokenizer st = new StringTokenizer(nextLine.toLowerCase(), " _&,.!?-;:\")#%*0123456789`([]");
					
					while(st.hasMoreTokens())
					{
						wordLine = st.nextToken();
						if(!myCollection.contains(wordLine))
						{
							myCollection.add(wordLine);
						}
						
						if(myMap.containsKey(wordLine))
						{
							myMap.put(wordLine, myMap.get(wordLine) + 1);
						}
						else
						{
							myMap.put(wordLine, 1);
						}
					}
				}
			}
			
			catch(IOException e)
			{
				System.out.println(e.getMessage());
			}
			
			finally
			{
				if(br != null)
					br.close();
			}
		}
			
		catch(FileNotFoundException e)
		{
			System.out.println(e.getMessage());
		}
		
		catch(IOException e)
		{
			System.out.println(e.getMessage());
		}	
	
		long stopTime = System.currentTimeMillis();
		double secondsElapsed = (((double)(stopTime - startTime)) / 1000);
		
		for(String item : myMap.keySet())
		{
			Word word = new Word(item, myMap.get(item));
			wordList.add(word);
		}
		
		Collections.sort(wordList);
		
		for(Word nextWord : wordList)
		{
			System.out.println(nextWord);
		}
		
		System.out.println("");
		System.out.println("The reading you selected was " + BookName);
		System.out.println("Total number of unique words: " + myCollection.size());
		System.out.println("Total time elapsed: " + secondsElapsed + " seconds");
		System.out.println("");
		}
		
	else
	{
		FileString = "";
		System.out.println("Sorry, your princess is in another castle... Try again, but this time select a value from the list!");
		System.out.println("");
	}
	
		System.out.println("Thank you for checking out my app!");
	}
}
