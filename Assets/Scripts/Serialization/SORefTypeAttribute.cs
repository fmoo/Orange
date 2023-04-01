using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor;

public class SORefTypeAttribute : PropertyAttribute {
    public System.Type type;

    public SORefTypeAttribute(System.Type type) {
        this.type = type;
    }
}
