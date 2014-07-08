/*
*
* @Author: Aubrey
* JS to calculate the points for the password string.
* 
*/















function contains_numbers(str) {
    if (str.replace(/[^0-9]/g, "").length > 0)
        return true;
    return false;
}
function contains_lowercase(str) {
    if (str.replace(/[^a-z]/g, "").length > 0)
        return true;
    return false;
}
function contains_uppercase(str) {
    if (str.replace(/[^A-Z]/g, "").length > 0)
        return true;
    return false;
}
function contains_puntuation(str) {
    if (str.replace(/[A-Za-z0-9]/g, "").length > 0)
        return true;
    return false;
}

/* points for combination */
function combo_points(str) {
    var counter = 0;
    if (contains_numbers(str))
        counter++;
    if (contains_lowercase(str))
        counter++;
    if (contains_uppercase(str))
        counter++;
    if (contains_puntuation(str))
        counter++;
    if (counter == 3)
        return 10;
    else if (counter == 4)
        return 20;
    else if (counter == 2)
        return 0;
    else
        return 0;
}

/* type points */
function typepoints(str) {
    var counter = 0;
    if (contains_lowercase(str) && contains_uppercase(str))
        counter += 15;
    if (contains_puntuation(str))
        counter += 15;
    if (contains_numbers(str))
        counter += 15;
    return counter;
}
function strong_enough(strength) {
    if (strength < 10)
        return false;
    return true;
}
/* check if the entry is all the same case */
function uniform(str) {

    var count = 0;

    //alert(str.replace(/[^A-Z]/g, "").length);

    if (str.replace(/[^A-Z]/g, "").length > 0)
        count++;
    //lowercase
    if (str.replace(/[^a-z]/g, "").length > 0)
        count++;
    //digits
    if (str.replace(/[^0-9]/g, "").length > 0)
        count++;

    if (str.replace(/[A-Za-z0-9]/g, "").length > 0)
        count++;

    if (count == 1) {
        return true;
    }
    else {
        return false;
    }
}
/* length points : max from length is 35 points */
function lengthPoints(str) {

    var points = 0;

    //0 to 4
    if (str.length < 6) {
        points = 0;
    }
    //6 to 7
    if (str.length < 8 && str.length > 5) {
        points += 7;
    }
    //8 to 12
    if (str.length < 13 && str.length > 7) {
        points += 19;
    }
    //longer than 12
    if (str.length > 12) {
        points += 35;
    }

    return points;

}