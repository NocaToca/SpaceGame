using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
So, to understand this and help me implement things I'm going to write out some stuff myself

An interesting thing about hexagons in 2d is that you can visually represent them as 3d cubes
Really, just make a hexagon and draw a line coming from every other vertex to the center (you should have 3 lines) and it looks like a cube!

So, to do this, we're going to actually have to traverse a 3d coordinate system made out of cubes instead of squares (so we're upping the dimension of the plane)

However, we only want cubes that make sense (this part is a little tricky to explain)
Basically, all the x y and z coordinates should add up to 0 for reasons I know but can't put into words. It's much much easier to look at a graph and see it yourself (Google hex grid coordinate system and look at the first result)

But in short terms, think of how our hex grid looks as cubes stacked on top of each other. For it to make sense, you always need to go up one in one axis and down one in another 

So Instead of having the typical 22 neighbors you'd normally have to move in any direction for the 3d plane we will only have 6
This means we CAN do this by forcing things to be 2d, but then diagonal movements will be more costly which is the whole reason we made a hexagonal system
We have to use the centers as a distinction point, or make it 3d. Since our board has no negative direction we have to do the latter as we have no way of telling if something is in the wrong direction otherwise

The only coordinate we can reliably increment is the x coordinate. We can only decrement the y coordinate which will move use diagonally up right and incrementing the z coordinate will send us diagonally up left
What we do need to recgonize though is the small problem this creates. When we do indeed make a board array of [5,5] the hexes will turn out like this:
    .....
   .....
  .....
 .....
.....
This is just because of how the hex coordinates work. I mentioned before that decrementing y will allow us to go up right, but incrementing z will do the same thing since x stays 0 in that coordinate. 
This does mean that we can't rely solely on indexes, and we have to create our own coordinates in this manner. Luckily, we can do this pretty reliabely.

If our board is like that and we want to make it like this: 
.....
 .....
.....
 .....
..... (keep in mind these are hexes so this arrangement makes sense)
We have to figure out this transformation. We have the equation:
x+y+z = 0
and we have f(x,y) = z
Along with the transformation(s):
(0,2) = -2 -> (-1, 2) -> -1 
(0,3) = -3 -> (-1, 3) -> -2
(0,4) = -4 -> (-2, 4) -> -2
We also know all grid lines have a similar transformation. 
It seems like a knowledge of linear algebra will actually help us find out what transformation is occuring here, but I'll explain it in terms I know how to explain

We can see that the z is forced to add 1 and the x is forced to subract one for every two lines up. Z also has to be -(y/2+y%2)
*/


public class HexBasedAStar
{
    //For practical purposes, I'm going to swap z and y
    public static Hex[] AStar(HexObject startingHex, HexObject endingHex, int boardHeight, int boardWidth){
        //first, lets make our board (remember its 3D technically)
        AStarGrid grid = new AStarGrid(boardWidth, boardHeight);

        //Now lets deal with our initial z coordinates. We know that z has to be -y at these points
        for(int i = 0; i < grid.grid.GetLength(0); i++){
            grid.grid[i, 0] = new AStarHex(0, -i, i, grid);
            //Now with our 6x6 Hex grid we have the points (0,0,0),(0,-1,1),(0,-2,2),(0,-3,3),(0,-4,4),(0,-5,5)
        }
        //That's all well and good, but lets now make the whole grid that's wrong!
        for(int z = 0; z < grid.grid.GetLength(0); z++){
            for(int x = 0; x < grid.grid.GetLength(1); x++){
                grid.grid[z, x] = new AStarHex(x, AStarHex.findY(x, z), z, grid);
            }
        }
        //Now lets write out every coordinate we have!
        /*                  (0,-5,5)(1,-6,5)(2,-7,5)(3,-8,5)(4,-9,5)(5,-10,5)
                        (0,-4,4)(1,-5,4)(2,-6,4)(3,-7,4)(4,-8,4)(5,-9,4)
                    (0,-3,3)(1,-4,3)(2,-5,3)(3,-6,3)(4,-7,3)(5,-8,3)
                (0,-2,2)(1,-3,2)(2,-4,2)(3,-5,2)(4,-6,2)(5,-7,2)
            (0,-1,1)(1,-2,1)(2,-3,1)(3,-4,1)(4,-5,1)(5,-6,1)
        (0,0,0)(1,-1,0)(2,-2,0)(3,-3,0)(4,-4,0)(5,-5,0)*/
        //We can see with this how our grid is laid out right now too.
        //Now, it would be nice to get our grid looking like this:
        /*  (-2,-3,5)(-1,-4,5)(0,-5,5)(1,-6,5)(2,-7,5)(3,-8,5)
        (-2,-2,4)(-1,-3,4)(0,-4,4)(1,-5,4)(2,-6,4)(3,-7,4)
            (-1,-2,3)(0,-3,3)(1,-4,3)(2,-5,3)(3,-6,3)(4,-7,3)
        (-1,-1,2)(0,-2,2)(1,-3,2)(2,-4,2)(3,-5,2)(4,-6,2)
            (0,-1,1)(1,-2,1)(2,-3,1)(3,-4,1)(4,-5,1)(5,-6,1)
        (0,0,0)(1,-1,0)(2,-2,0)(3,-3,0)(4,-4,0)(5,-5,0)*/

        //However, while our board is in this state we can oh so easily get which AStarHex the startingHex cooresponds to!
        AStarHex start = new AStarHex(0,0,0, grid);
        Vector2 XandY = Board.FindHexCoordsInBoard(startingHex.hex);
        for(int z = 0; z < grid.grid.GetLength(0); z++){
            for(int x = 0; x < grid.grid.GetLength(1); x++){
                if(x == XandY.x && z == XandY.y){
                    start = new AStarHex(x, AStarHex.findY(x,z), z, grid);
                    grid.grid[z, x] = start;

                }
            }
        }

        //Let's just do the same thing with the ending hex here
        AStarHex ending = new AStarHex(0,0,0, grid);
        XandY = Board.FindHexCoordsInBoard(endingHex.hex);
        for(int z = 0; z < grid.grid.GetLength(0); z++){
            for(int x = 0; x < grid.grid.GetLength(1); x++){
                if(x == XandY.x && z == XandY.y){
                    ending = new AStarHex(x, AStarHex.findY(x,z), z, grid);
                    grid.grid[z, x] = ending;
                }
            }
        }

        //It is now a lot easier to see the transformation we're trying to do. In sort, we're shifting the x over a certain amount, causing us to need to shift the y
        //it looks like y has to start at -([z/2]). Since we already have the board, lets shift the y over that amount with each point
        for(int z = 0; z < grid.grid.GetLength(0); z++){
            for(int x = 0; x < grid.grid.GetLength(1); x++){
                //Lets first work through what each coord is right now
                int coordX = grid.grid[z,x].x;
                int coordY = grid.grid[z,x].y;
                int coordZ = grid.grid[z,x].z;
                //Remember we don't have to change z; it's static (a better term would be constant) in this situation, but we need it to get y
                //Upon testing, z seems to be raised 1
                //coordZ -= 1;

                //Now lets apply the transformation to the y

                coordY = coordY - (-1)*(coordZ/2);// + coordZ%2);
                //now we can just get the x from those two coordinates
                //coordY += 1;
                coordX = AStarHex.findY(coordY, coordZ); //Funny, but findY is more like "find last variable"

                //Now let's update what we got
                grid.grid[z,x].x = coordX;
                grid.grid[z,x].y = coordY;
                grid.grid[z,x].z = coordZ;

                // int save = grid.grid[z,x].x;
                // grid.grid[z,x].x = grid.grid[z,x].y;
                // grid.grid[z,x].y = save;


            }
        }

        //Now that we have this done, we can work on thinking about how we determine if a cell has a neighbor. From the look of it, a cell has 6 neighbors max, 3 min. Let's think why
        //Well, lets look at the point (2,-4,2)
        /*
        
            (1,-4,3)(2,-5,3)
        (1,-3,2)(2,-4,2)(3,-5,2)
            (2,-3,1)(3,-4,1)
        */
        //its neighbors are: (1,-4,3)(2,-5,3)(1,-3,2)(3,-5,2)(2,-3,1)(3,-4,1)
        //That's all well and good, but how do we get this based off of indexes? Well, if you know how arrays work, ours currently looks like this:
        /*  
        (-2,-3,5)(-1,-4,5)(0,-5,5)(1,-6,5)(2,-7,5)(3,-8,5)
        (-2,-2,4)(-1,-3,4)(0,-4,4)(1,-5,4)(2,-6,4)(3,-7,4)
        (-1,-2,3)(0,-3,3)(1,-4,3)(2,-5,3)(3,-6,3)(4,-7,3)
        (-1,-1,2)(0,-2,2)(1,-3,2)(2,-4,2)(3,-5,2)(4,-6,2)
        (0,-1,1)(1,-2,1)(2,-3,1)(3,-4,1)(4,-5,1)(5,-6,1)
        (0,0,0)(1,-1,0)(2,-2,0)(3,-3,0)(4,-4,0)(5,-5,0)
        */
        //Let's look for (2,-4,2) and it's neighbors by bracketing them
        /*  
        (-2,-3,5)(-1,-4,5)(0,-5,5)(1,-6,5)(2,-7,5)(3,-8,5)
        (-2,-2,4)(-1,-3,4)(0,-4,4)(1,-5,4)(2,-6,4)(3,-7,4)
        (-1,-2,3)(0,-3,3)[(1,-4,3)][(2,-5,3)](3,-6,3)(4,-7,3)
        (-1,-1,2)(0,-2,2)[(1,-3,2)](2,-4,2)[(3,-5,2)](4,-6,2)
        (0,-1,1)(1,-2,1)[(2,-3,1)][(3,-4,1)](4,-5,1)(5,-6,1)
        (0,0,0)(1,-1,0)(2,-2,0)(3,-3,0)(4,-4,0)(5,-5,0)
        */
        //Now we can see that we can look (if our 2d array is in form x,y) (x+1,y)(x-1,y)(x,y+1)(x,y-1)(x-1,y+1)(x-1,y-1)
        //This is interesting, and seeing that x is only subtracted when we do plus or minus y suggests that there may be more to look at. Let's look at (2,-5,3):
        /*
            (1,-5,4)(2,-6,4)
        (1,-4,3)(2,-5,3)(3,-6,3)
            (2,-4,2)(3,-5,2)
        */
        //Let's bracket the nieghbors of (2,-5,3) now by bracketing them too
        /*  
        (-2,-3,5)(-1,-4,5)(0,-5,5)(1,-6,5)(2,-7,5)(3,-8,5)
        (-2,-2,4)(-1,-3,4)(0,-4,4)[(1,-5,4)][(2,-6,4)](3,-7,4)
        (-1,-2,3)(0,-3,3)[(1,-4,3)](2,-5,3)[(3,-6,3)](4,-7,3)
        (-1,-1,2)(0,-2,2)(1,-3,2)[(2,-4,2)][(3,-5,2)](4,-6,2)
        (0,-1,1)(1,-2,1)(2,-3,1)(3,-4,1)(4,-5,1)(5,-6,1)
        (0,0,0)(1,-1,0)(2,-2,0)(3,-3,0)(4,-4,0)(5,-5,0)
        */
        //We can see that we can look (if our 2d array is in form x,y) (x+1,y)(x-1,y)(x,y+1)(x,y-1)(x+1,y+1)(x+1,y-1)
        //As expected, it matters what row the original hex is in, or the "y" of the array. If the y is even, we're going to add x and plus or minus y, otherwise we'd subtract x!
        //So lets implement this in AddNeighbors()

        string info = "";

        for(int z = grid.grid.GetLength(0)-1; z >= 0; z--){
            for(int x = 0; x < grid.grid.GetLength(1); x++){
                info = info +"(" + grid.grid[z,x].x + "," +  grid.grid[z,x].y + "," + grid.grid[z,x].z + ")";
            }
            info = info + "\n";
        }
        //Debug.Log(info);
        for(int z = 0; z < grid.grid.GetLength(0); z++){
            for(int x = 0; x < grid.grid.GetLength(1); x++){
                grid.grid[z, x].AddNeighbors(z, boardWidth, boardHeight);
            }
        }

        //Now we have the neighbors, so let's get cracking

        //Now, with this done, we have a 2d plane of the 3d coordinates, so we can techincally go about normal A* with this whack grid. Let's start that
        List<AStarHex> openCells = new List<AStarHex>();
        List<AStarHex> closedCells = new List<AStarHex>();
        List<AStarHex> path = new List<AStarHex>();

        //But now we need to make the Hex that we get as the starting hex and transfer it to the hex our AStar algorithm can read
        //Hopefully these are right
        //start = grid.grid[0,0];
        //ending = grid.grid[5,5];
        
        openCells.Add(start);

        while(openCells.Count != 0){
            //Debug.Log("Run!");
            int lowestIndex = 0;
            for(int i = 0; i < openCells.Count; i++){
                if(openCells[i].f < openCells[lowestIndex].f){
                    lowestIndex = i;
                }
            }

            AStarHex current = openCells[lowestIndex];
            List<AStarHex> currentNeighbors = current.neighbors; 
            //Debug.Log(currentNeighbors.Count);

            // if(current == ending){
            //     //Do finishing things here
            //     AStarHex temp = current;
            //     while(temp.hasPrev){
            //         path.Add(temp);
            //         temp = temp.previous;
            //     }
            //     Debug.Log("Break1!");
            //     break;
            //} else 
            if(currentNeighbors.Contains(ending)){
                int indexOf = 0;
                for(int i = 0; i < currentNeighbors.Count; i++){

                    if(currentNeighbors[i] == ending){

                        indexOf = i;

                    }

                }
                currentNeighbors[indexOf].previous = current;
                currentNeighbors[indexOf].hasPrev = true;

                AStarHex temp = currentNeighbors[indexOf];
                path.Add(temp);
                while(temp.hasPrev){
                    
                    temp = temp.previous;
                    path.Add(temp);
                }
                //Debug.Log("Break2!");
                break;
                //Do finsihing things here
            }

            openCells.Remove(current);
            closedCells.Add(current);

            for(int i = 0; i < currentNeighbors.Count; i++){
                AStarHex neighbor = currentNeighbors[i];
                int tempG = current.g + 1;
                if(!closedCells.Contains(neighbor)){
                    
                    if(openCells.Contains(neighbor)){
                        if(tempG < neighbor.g){
                            neighbor.g = tempG;
                            //neighbor.previous = current;
                            //neighbor.hasPrev = true;
                            
                        }
                    } else {
                        neighbor.g = tempG;
                        openCells.Add(neighbor);
                        
                    }
                

                    neighbor.h = AStarHex.distance(neighbor, ending);
                    neighbor.f = neighbor.g + neighbor.h;
                    neighbor.previous = current;
                    neighbor.hasPrev = true;
                }

                
            }

        }
        Hex[] hexesInPath = new Hex[path.Count];
        for(int i = 0; i < path.Count; i++){
            hexesInPath[i] = AStarHex.AStarHexToHex(path[i]).hex;
        }
        

        return hexesInPath;

    }
}

public class AStarGrid{
    public AStarHex [,] grid; //Our grid only has to be in 2d (x and z) as the y we can get from the other two (plus this is how many cells we actually have)
    public int w;
    public int h;

    public AStarGrid(int w, int h){
        this.w = w;
        this.h = h;
        grid = new AStarHex[h,w];
    }

    public AStarHex GetCell(int x, int y){
        return grid[y,x];
    }
}

public class AStarHex{
    public List<AStarHex> neighbors = new List<AStarHex>();
    public bool isStart;
    public bool isEnd;
    public int g;
    public float h;
    public float f;
    public int x;
    public int y;
    public int z;
    public AStarHex previous;
    public bool hasPrev;
    AStarGrid grid;

    public AStarHex(int x, int y, int z, AStarGrid grid){
        this.x = x;
        this.y = y;
        this.z = z;
        g = 0;
        f = 0;
        h = 0; 
        hasPrev = false;
        this.grid = grid;
    }

    public static int findY(int x, int z){
        return -x - z;
    }

    public void AddNeighbors(int z, int w, int h){
        int specialXMultiplier = -1;
        if(z % 2 == 1){
            specialXMultiplier = 1;
        }
        //Now we can just check if all of the cells exist, and if they do, add them!

        Vector2 vect = ReturnToOldCoords(this);

        int y = (int)vect.y;
        int x = (int)vect.x;

        //Debug.Log("(" + x + "," + y + ")");

        if(y < h-1){
            neighbors.Add(grid.GetCell(x, y+1));
            if(specialXMultiplier == -1){
                if(x > 0){
                    neighbors.Add(grid.GetCell(x-1, y+1));
                }
            } else
            if(specialXMultiplier == 1){
                if(x < w-1){
                    neighbors.Add(grid.GetCell(x+1, y+1));
                }
            }
        }
        if(y > 0){
            neighbors.Add(grid.GetCell(x, y-1));
            if(specialXMultiplier == -1){
                if(x > 0){
                    neighbors.Add(grid.GetCell(x-1, y-1));
                }
            } else
            if(specialXMultiplier == 1){
                if(x < w-1){
                    neighbors.Add(grid.GetCell(x+1, y-1));
                }
            }
        }
        if(x < w-1){
            neighbors.Add(grid.GetCell(x+1, y));
        }
        if(x > 0){
            neighbors.Add(grid.GetCell(x-1, y));
        }
    }

    public static float distance(AStarHex hex1, AStarHex hex2){
        float z = hex1.z-hex2.z;
        float y = hex1.y-hex2.y;
        float x = hex1.x-hex2.x;
        return Mathf.Sqrt(x*x+y*y+z*z);
    }

    public static Vector2 ReturnToOldCoords(AStarHex hex){
        int y = hex.y - (hex.z/2);// + hex.z%2);
        int x = -y-hex.z;
        return new Vector2(x,hex.z);
    }

    public static HexObject AStarHexToHex(AStarHex hex){
        return Board.GetHex(ReturnToOldCoords(hex));
    }
}
